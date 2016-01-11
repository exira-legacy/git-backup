namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("git-backup")>]
[<assembly: AssemblyProductAttribute("Exira.GitBackup")>]
[<assembly: AssemblyDescriptionAttribute("Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule")>]
[<assembly: AssemblyVersionAttribute("0.4.5")>]
[<assembly: AssemblyFileVersionAttribute("0.4.5")>]
[<assembly: AssemblyMetadataAttribute("githash","539b40baa96c733991caeefc5220c3a20bcc8667")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.4.5"
