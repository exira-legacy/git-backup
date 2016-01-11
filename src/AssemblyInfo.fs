namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("git-backup")>]
[<assembly: AssemblyProductAttribute("Exira.GitBackup")>]
[<assembly: AssemblyDescriptionAttribute("Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule")>]
[<assembly: AssemblyVersionAttribute("0.4.6")>]
[<assembly: AssemblyFileVersionAttribute("0.4.6")>]
[<assembly: AssemblyMetadataAttribute("githash","2cc5ed25dbf74a86cebec7159efe6fb8b5919755")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.4.6"
