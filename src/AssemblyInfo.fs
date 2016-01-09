namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("git-backup")>]
[<assembly: AssemblyProductAttribute("Exira.GitBackup")>]
[<assembly: AssemblyDescriptionAttribute("Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule")>]
[<assembly: AssemblyVersionAttribute("0.2.3")>]
[<assembly: AssemblyFileVersionAttribute("0.2.3")>]
[<assembly: AssemblyMetadataAttribute("githash","4d0bb96b1314c5b30d3b3699e0db0a24b6dbb1eb")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.2.3"
