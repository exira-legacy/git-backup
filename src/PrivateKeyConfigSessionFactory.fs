namespace Exira.GitBackup

module internal PrivateKeyConfigSessionFactory =

    open System
    open System.IO
    open NGit.Transport
    open NGit.Util
    open NSch
    open Sharpen

    type PrivateKeyConfigSessionFactory(privateKeyPath) =
        inherit JschConfigSessionFactory()

        do
            Environment.SetEnvironmentVariable("GIT_SSH", "", EnvironmentVariableTarget.Process)

        override __.Configure(hc: OpenSshConfig.Host, session: Session) =
            let config = Properties()
            config.["StrictHostKeyChecking"] <- "no"
            config.["PreferredAuthentications"] <- "publickey"
            session.SetConfig config

            let jsch = base.GetJSch(hc, FS.DETECTED)
            jsch.AddIdentity("KeyPair", File.ReadAllBytes privateKeyPath, null, null)
