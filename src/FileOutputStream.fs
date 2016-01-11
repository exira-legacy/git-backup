namespace Exira.GitBackup

module internal FileOutputStream =

    // Minimally ported from https://github.com/mono/ngit/blob/master/Sharpen/Sharpen/FileOutputStream.cs

    open System.IO
    open Sharpen

    type FileOutputStream(file) as self =
        inherit OutputStream()

        do
            self.Wrapped <- File.Open(file, FileMode.Create, FileAccess.Write) :> Stream
