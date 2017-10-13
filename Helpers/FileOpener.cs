using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MVVMHelper.Helpers
{
    /// <summary>
    /// Gestisce l'apertura di un file usando l'applicazione predefinita del sistema operativo
    /// </summary>
    public static class FileOpener
    {
        /// <summary>
        /// Apre il file utilizzando l'applicazione predefinita
        /// </summary>
        /// <param name="filePath">path del file da aprire</param>
        public static Process OpenFile( string filePath )
        {
            return OpenFile( filePath, false );
        }

        /// <summary>
        /// Apre il file utilizzando l'applicazione predefinita
        /// </summary>
        /// <param name="filePath">path del file da aprire</param>
        /// <param name="readOnly">apre il file in modalità readonly</param>
        public static Process OpenFile( string filePath, bool readOnly )
        {
            //se devo aprire il file in modalità readonly
            if( readOnly )
            { 
                //setto gli attributi readonly al file da aprire
                FileAttributes curAttributes = File.GetAttributes( filePath );
                File.SetAttributes( filePath, curAttributes | FileAttributes.ReadOnly );
            }

            //lancio un processo per aprire il file
            Process target = Process.Start( filePath );
            //se il processo non è null setto che può mandare eventi al chiamante
            if( target != null )
			    target.EnableRaisingEvents = true;
            //ritorno il processo
			return target;
        }
    }
}
