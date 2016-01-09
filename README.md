# git-backup [![NuGet Status](http://img.shields.io/nuget/v/Exira.GitBackup.svg?style=flat)](https://www.nuget.org/packages/Exira.GitBackup/)

## Exira.GitBackup

Exira.GitBackup is a console application which backs up Git repositories on a rotating schedule.

## Usage

 * Download the latest release from [the GitHub releases page](https://github.com/exira/git-backup/releases)
 * Unzip somewhere on a machine where you can put it in a scheduled task

 * Edit `Backup.yaml` using:
  * NumberOfDailyBackups: `# daily backups to keep`, e.g.: `7`
  * NumberOfWeeklyBackups: `# weekly backups to keep`, e.g.: `4`
  * NumberOfMonthlyBackups: `# monthly backups to keep`, e.g.: `6`
  * NumberOfYearlyBackups: `# yearly backups to keep`, e.g.: `2`
  * BackupRoot: `Path where backups will be stored`, e.g.: `C:\git-backup\`
  * Repositories: `A list of all repositories to back up`
    * Name: `Unique name of repository, used as backup key`, e.g.: `git-backup`
    * Url: `Git location of repository, supports https and ssh`, e.g.: `ssh://git@github.com:exira/git-backup.git`
    * (optional) PrivateKey: `Path to private key to be used in case of SSH, in OpenSSH format`, e.g.: `C:\git-backup\keys\git-backup.priv`

 * Run `git-backup.exe` on a fixed time (put it in a scheduled task)

 * For all repositories a bundle will be created and bundles which do not meet your retention settings will be deleted

## Information

##### You can check what is in your bundle:
`git bundle list-heads C:\git-backup\2016.01.09-git-backup.git`

##### To send it by email or any dumb transport, and in the repository on the other side:
`git bundle verify C:\git-backup\2016.01.09-git-backup.git`

##### After moving the bundle on any location you can get a new repository by:
`git clone -b master C:\git-backup\2016.01.09-git-backup.git newrepo`

## Cloning

```git clone git@github.com:exira/git-backup.git -c core.autocrlf=input```

## Copyright

Copyright © 2016 Cumps Consulting BVBA / Exira and contributors.

## License

git-backup is licensed under [BSD (3-Clause)](http://choosealicense.com/licenses/bsd-3-clause/ "Read more about the BSD (3-Clause) License"). Refer to [LICENSE.txt](https://github.com/exira/git-backup/blob/master/LICENSE.txt) for more information.
