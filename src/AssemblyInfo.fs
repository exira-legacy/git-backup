namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("git-backup")>]
[<assembly: AssemblyProductAttribute("Exira.GitBackup")>]
[<assembly: AssemblyDescriptionAttribute("Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule")>]
[<assembly: AssemblyVersionAttribute("0.5.7")>]
[<assembly: AssemblyFileVersionAttribute("0.5.7")>]
[<assembly: AssemblyMetadataAttribute("githash","6e7088b5aa5de027d7f16cce96e42d77914a6c80")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.5.7"
