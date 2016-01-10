namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("git-backup")>]
[<assembly: AssemblyProductAttribute("Exira.GitBackup")>]
[<assembly: AssemblyDescriptionAttribute("Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule")>]
[<assembly: AssemblyVersionAttribute("0.3.4")>]
[<assembly: AssemblyFileVersionAttribute("0.3.4")>]
[<assembly: AssemblyMetadataAttribute("githash","277a4bbec984dedcb259777cebb13e409705b9d7")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.3.4"
