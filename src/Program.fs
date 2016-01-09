open FSharp.Configuration
open System
open System.IO
open System.Reflection
open System.Globalization
open Exira.ErrorHandling
open Sharpen
open NGit.Api
open NGit.Transport
open NGit
open FileOutputStream
open PrivateKeyConfigSessionFactory

let executablePath = Assembly.GetEntryAssembly().Location |> Path.GetDirectoryName
let configPath = Path.Combine(executablePath, "Backup.yaml")

type BackupConfig = YamlConfig<"Backup.yaml">
let backupConfig = BackupConfig()
backupConfig.Load configPath

let toMap dictionary =
    dictionary :> seq<_>
    |> Seq.map (|KeyValue|)
    |> Map.ofSeq

type Errors =
    | FailedToBundleRepositories of Errors list
    | FailedToBundleRepository of string * exn
    | FailedToRetrieveBundles of string * exn
    | FailedToDeleteBundle of string * exn
    | FailedToDeleteBundles of Errors list

let format errors =
    let rec formatError error =
        match error with
        | FailedToBundleRepository (name, ex) -> sprintf "Could not create bundle for %s: %s" name (ex.ToString())
        | FailedToRetrieveBundles (name, ex) -> sprintf "Could not find bundles for %s: %s" name (ex.ToString())
        | FailedToDeleteBundle (bundle, ex) -> sprintf "Could not delete bundle %s: %s" bundle (ex.ToString())
        | FailedToBundleRepositories e
        | FailedToDeleteBundles e
            -> e |> List.map formatError |> String.concat Environment.NewLine

    errors
    |> List.map formatError

let printBundle message bundle =
    if backupConfig.Backup.Debug then
        printfn "[%s] Bundle: %s" message bundle
    else ()

let printBundles message (bundles: seq<string>) =
    if backupConfig.Backup.Debug then
        bundles
        |> Seq.sortBy (fun s ->
            let date = s.Split('-') |> Array.head
            DateTime.ParseExact(date, "yyyy.MM.dd", CultureInfo.InvariantCulture))
        |> Seq.iter (printBundle message)
    else ()

let daysToKeep =
    let today = DateTime.Today
    let numberOfDailyBackups = backupConfig.Backup.NumberOfDailyBackups
    let numberOfWeeklyBackups = backupConfig.Backup.NumberOfWeeklyBackups
    let numberOfMonthlyBackups = backupConfig.Backup.NumberOfMonthlyBackups
    let numberOfYearlyBackups = backupConfig.Backup.NumberOfYearlyBackups

    let firstOfMonth = DateTime(today.Year, today.Month, 1)
    let firstOfYear = DateTime(today.Year, 1, 1)

    let daily = seq { for n in 0 .. numberOfDailyBackups - 1 do yield today.AddDays (float n * -1.) }

    let weekly =
        Seq.initInfinite (fun i -> today.AddDays (float i * -1.))
        |> Seq.filter (fun date -> date.DayOfWeek = DayOfWeek.Sunday)
        |> Seq.take numberOfWeeklyBackups

    let monthly = seq { for n in 0 .. numberOfMonthlyBackups - 1 do yield firstOfMonth.AddMonths (n * -1) }

    let yearly = seq { for n in 0 .. numberOfYearlyBackups - 1 do yield firstOfYear.AddYears (n * -1) }

    daily
    |> Seq.append weekly
    |> Seq.append monthly
    |> Seq.append yearly
    |> Seq.distinct
    |> Seq.toList

let formatLocation (repoLocation: Uri) =
    if (repoLocation.Scheme = "ssh") then
        let tempLocation = repoLocation.ToString()

        let tempLocation =
            if (repoLocation.IsDefaultPort) then
                tempLocation.Replace((sprintf "%s/" repoLocation.Host), (sprintf "%s:22/" repoLocation.Host))
            else tempLocation

        let tempLocation =
            if (repoLocation.UserInfo.Contains ":") then tempLocation
            else tempLocation.Replace((sprintf "%s@" repoLocation.UserInfo), (sprintf "%s:x@" repoLocation.UserInfo))

        tempLocation.ToString()
    else repoLocation.ToString()

let bundleRepository (repository: BackupConfig.Backup_Type.Repositories_Item_Type) =
    let init repoName =
        Git.Init()
           .SetBare(true)
           .SetDirectory(FilePath repoName)
           .Call()

    let fetch repoLocation privateKeyPath (git: Git) =
        let factory = PrivateKeyConfigSessionFactory privateKeyPath
        SshSessionFactory.SetInstance factory

        git.Fetch()
           .SetRemote(repoLocation)
           .SetRefSpecs([| RefSpec "+refs/*:refs/*" |])
           .Call() |> ignore
        git

    let bundle (bundleLocation: string) (git: Git) =
        let repo = git.GetRepository()
        let bundle = BundleWriter repo
        repo.GetAllRefs()
        |> toMap
        |> Map.iter (fun _ value -> bundle.Include value)

        bundleLocation
        |> Path.GetDirectoryName
        |> Directory.CreateDirectory
        |> ignore

        use bundleOutput = new FileOutputStream(bundleLocation)
        bundle.WriteBundle(NullProgressMonitor.INSTANCE, bundleOutput)
        bundleOutput.Close()
        git

    let remove repoName (git: Git) =
        let repo = git.GetRepository()
        repo.Close()
        repo.ObjectDatabase.Close()

        Directory.GetFiles(repoName, "*", SearchOption.AllDirectories)
        |> Seq.iter (fun file -> File.SetAttributes(file, FileAttributes.Normal))
        Directory.Delete(repoName, true)

    let repoName = repository.Name
    let repoLocation = repository.Url |> formatLocation
    let repoKey = repository.PrivateKey
    let bundleLocation = Path.Combine [| backupConfig.Backup.BackupRoot; repository.Name; sprintf "%s-%s.git" (DateTime.Today.ToString("yyyy.MM.dd")) repoName |]

    try
        init repoName
        |> fetch repoLocation repoKey
        |> bundle bundleLocation
        |> remove repoName

        succeed bundleLocation
    with
    | ex -> fail [FailedToBundleRepository (repository.Name, ex)]

let getBundles (repository: BackupConfig.Backup_Type.Repositories_Item_Type) =
    try
        let backupFilter = sprintf "*-%s.git" repository.Name
        let bundleLocation = Path.Combine [| backupConfig.Backup.BackupRoot; repository.Name |]
        Directory.GetFiles(bundleLocation, backupFilter) |> succeed
    with
    | ex -> fail [FailedToRetrieveBundles (repository.Name, ex)]

let pruneBackups (backups: seq<string>) =
    let deleteBundle bundle =
        try
            File.Delete bundle
            succeed bundle
        with
        | ex -> fail [FailedToDeleteBundle (bundle, ex)]

    let prunedBackups =
        backups
        |> Seq.filter (fun s ->
            let date = (Path.GetFileNameWithoutExtension s).Split('-') |> Array.head
            daysToKeep |> List.contains (DateTime.ParseExact(date, "yyyy.MM.dd", CultureInfo.InvariantCulture)) |> not)
        |> Seq.map deleteBundle
        |> Seq.toList

    let failures = prunedBackups |> List.choose failureOnly |> List.collect id
    let success = prunedBackups |> List.choose successOnly

    if List.isEmpty failures then
        succeed success
    else
        printBundles "Bundles pruned" success
        fail [FailedToDeleteBundles failures]

let backupRepository (repository: BackupConfig.Backup_Type.Repositories_Item_Type) =
    repository
    |> bundleRepository
    |> map (printBundle "Bundle created" |> tee)
    |> bind (fun _ -> getBundles repository)
    |> map (printBundles "Bundles retrieved" |> tee)
    |> bind pruneBackups
    |> map (printBundles "Bundles pruned" |> tee)

let backup (repositories: seq<BackupConfig.Backup_Type.Repositories_Item_Type>) =
    let backups =
        repositories
        |> Seq.map backupRepository
        |> Seq.toList

    let failures = backups |> List.choose failureOnly |> List.collect id
    let success = backups |> List.choose successOnly |> Seq.collect id

    if List.isEmpty failures then succeed success
    else fail [FailedToBundleRepositories failures]

let backupFailed errors =
    errors
    |> format
    |> String.concat Environment.NewLine
    |> printfn "Errors:%s %s" Environment.NewLine

[<EntryPoint>]
let main _ =
    let backups =
        backupConfig.Backup.Repositories
        |> backup

    match backups with
    | Success _ -> 0
    | Failure errors ->
        backupFailed errors
        -1
