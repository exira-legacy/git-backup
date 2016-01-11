﻿namespace Exira.GitBackup

module internal Configuration =
    open System.IO
    open System.Reflection
    open FSharp.Configuration

    let entryAssembly = Assembly.GetEntryAssembly()
    let executablePath = entryAssembly.Location |> Path.GetDirectoryName
    let configPath = Path.Combine(executablePath, "Backup.yaml")

    type BackupConfig = YamlConfig<"Backup.yaml">
    let backupConfig = BackupConfig()
    backupConfig.LoadAndWatch configPath |> ignore
