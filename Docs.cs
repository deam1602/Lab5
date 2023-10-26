using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using RsaEncryptionSample;
using System.Numerics;

namespace RSA
{
    public class Docs
    {
        
        public static void BuscarArchivoCifrar(long dpi,BigInteger ClaveP)//aquí se agrega la llave
        {
            RSAA rsaa = new RSAA();
            string carpeta = @"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\RSA miller\inputs";//ruta carpeta con cartas
            string ruta = @"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\RSA miller\Personas agregadas.txt";
            long buscado = dpi;
            string llave = "millave";
             List<string> DPIs = new List<string>();
            // Obtener los nombres de los archivos en la carpeta
            string[] nombresArchivos = Directory.GetFiles(carpeta);

            // Iterar sobre cada nombre de archivo
            foreach (string nombreArchivo in nombresArchivos)
            {
                // Obtener solo el nombre del archivo sin la ruta ni la extensión
                string nombreSinExtension = Path.GetFileNameWithoutExtension(nombreArchivo);
                string[] dpiEnNombre = nombreSinExtension.Split('-');
                long dpiInt = Convert.ToInt64(dpiEnNombre[1]);
                DPIs.Add(dpiInt.ToString());
                // Verificar si el nombre del archivo es igual al dpi buscado
                if (dpiInt == buscado)
                {
                    try
                    {
                        // Leer el contenido del archivo y guardarlo en un string
                        string contenido = File.ReadAllText(nombreArchivo)+ "\n" + ClaveP;
                        Console.WriteLine($"\n\nConversación {nombreSinExtension} cifrada:\n");
                        Cifrado(nombreSinExtension, contenido, llave, '-');


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al leer el archivo {nombreArchivo}: {ex.Message}");
                    }
                }
            }

            File.WriteAllLines(ruta, DPIs);
        }

        public static void Cifrado(string nombreArchivo, string entrada, string llave, char relleno)
        {
            entrada = (entrada.Length % llave.Length == 0) ? entrada : entrada.PadRight(entrada.Length - (entrada.Length % llave.Length) + llave.Length, relleno);
            StringBuilder salida = new StringBuilder();
            int totalCaracteres = entrada.Length;
            int totalColumnas = llave.Length;
            int totalFilas = (int)Math.Ceiling((double)totalCaracteres / totalColumnas);
            char[,] filaCaracteres = new char[totalFilas, totalColumnas];
            char[,] colCaracteres = new char[totalColumnas, totalFilas];
            char[,] colOrdenadas = new char[totalColumnas, totalFilas];
            int filaActual, columnaActual, i, j;
            int[] mezclarIndexes = ObtenerIndex(llave);

            for (i = 0; i < totalCaracteres; ++i)
            {
                filaActual = i / totalColumnas;
                columnaActual = i % totalColumnas;
                filaCaracteres[filaActual, columnaActual] = entrada[i];
            }

            for (i = 0; i < totalFilas; ++i)
                for (j = 0; j < totalColumnas; ++j)
                    colCaracteres[j, i] = filaCaracteres[i, j];

            for (i = 0; i < totalColumnas; ++i)
                for (j = 0; j < totalFilas; ++j)
                    colOrdenadas[mezclarIndexes[i], j] = colCaracteres[i, j];

            for (i = 0; i < totalCaracteres; ++i)
            {
                filaActual = i / totalFilas;
                columnaActual = i % totalFilas;
                salida.Append(colOrdenadas[filaActual, columnaActual]);
            }
            
            EscribirCifrado(nombreArchivo, salida.ToString());
            Console.WriteLine(salida.ToString());
        }

        public static void EscribirCifrado(string nombreArchivo, string texto)
        {
            string contenido = texto;

            string rutaArchivo = @$"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\RSA miller\Conversaciones Cifradas\{ nombreArchivo}-cifrado.txt";

            try
            {
                // Crear un StreamWriter y escribir el contenido en el archivo
                using (StreamWriter writer = new StreamWriter(rutaArchivo))
                {
                    writer.Write(contenido);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al escribir en el archivo: {ex.Message}");
            }
        }

        public static void BuscarArchivoDescifrar(long dpi)
        {
            string carpeta = @"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\RSA miller\Conversaciones Cifradas"; // Cambia esto por la ruta de tu carpeta
            long buscado = dpi; // Cambia esto por el número entero que quieres verificar 
            string llave = "millave";
            // Obtener los nombres de los archivos en la carpeta
            string[] nombresArchivos = Directory.GetFiles(carpeta);

            // Iterar sobre cada nombre de archivo
            foreach (string nombreArchivo in nombresArchivos)
            {
                // Obtener solo el nombre del archivo sin la ruta ni la extensión
                string nombreSinExtension = Path.GetFileNameWithoutExtension(nombreArchivo);
                string[] dpiEnNombre = nombreSinExtension.Split('-');
                long dpiInt = Convert.ToInt64(dpiEnNombre[1]);

                // Verificar si el nombre del archivo es igual al dpi buscado
                if (dpiInt == buscado)
                {
                    try
                    {
                        // Leer el contenido del archivo y guardarlo en un string
                        string contenido = File.ReadAllText(nombreArchivo);
                        Console.WriteLine($"\n\nCarta {nombreSinExtension} descifrada:\n");
                        Descifrado(contenido, llave);


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al leer el archivo {nombreArchivo}: {ex.Message}");
                    }
                }
            }

        }

        public static void Descifrado(string entrada, string llave)
        {
            StringBuilder salida = new StringBuilder();
            int totalCaracteres = entrada.Length;
            int totalColumnas = (int)Math.Ceiling((double)totalCaracteres / llave.Length);
            int totalFilas = llave.Length;
            char[,] filaCaracteres = new char[totalFilas, totalColumnas];
            char[,] colCaracteres = new char[totalColumnas, totalFilas];
            char[,] colDesordenadas = new char[totalColumnas, totalFilas];
            int filaActual, columnaActual, i, j;
            int[] mezclarIndexes = ObtenerIndex(llave);

            for (i = 0; i < totalCaracteres; ++i)
            {
                filaActual = i / totalColumnas;
                columnaActual = i % totalColumnas;
                filaCaracteres[filaActual, columnaActual] = entrada[i];
            }

            for (i = 0; i < totalFilas; ++i)
                for (j = 0; j < totalColumnas; ++j)
                    colCaracteres[j, i] = filaCaracteres[i, j];

            for (i = 0; i < totalColumnas; ++i)
                for (j = 0; j < totalFilas; ++j)
                    colDesordenadas[i, j] = colCaracteres[i, mezclarIndexes[j]];

            for (i = 0; i < totalCaracteres; ++i)
            {
                filaActual = i / totalFilas;
                columnaActual = i % totalFilas;
                salida.Append(colDesordenadas[filaActual, columnaActual]);
            }

            Console.WriteLine(salida.ToString());
        }

        private static int[] ObtenerIndex(string llave)
        {
            int tamaniollave = llave.Length;
            int[] indexes = new int[tamaniollave];
            List<KeyValuePair<int, char>> llaveOrdenada = new List<KeyValuePair<int, char>>();
            int i;

            for (i = 0; i < tamaniollave; ++i)
                llaveOrdenada.Add(new KeyValuePair<int, char>(i, llave[i]));

            llaveOrdenada.Sort(
                delegate (KeyValuePair<int, char> par1, KeyValuePair<int, char> pair2) {
                    return par1.Value.CompareTo(pair2.Value);
                }
            );

            for (i = 0; i < tamaniollave; ++i)
                indexes[llaveOrdenada[i].Key] = i;

            return indexes;
        }
    }
}
