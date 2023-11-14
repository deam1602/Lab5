using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Numerics;

namespace RSA
{
    public class Login
    {
        List<Persona> ListaUsuarios = new List<Persona>();
        List<string> reclutadores = new List<string>();
        List<string> UsuarioContra = new List<string>();
        List<string> ContraCifrada = new List<string>();

        public void leerArchivo()
        {
            string filePath = @"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\Lab4y5\input.csv"; // Ruta archivo
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        string action = parts[0].Trim();
                        string dataJson = parts[1].Trim();

                        Persona person = Newtonsoft.Json.JsonConvert.DeserializeObject<Persona>(dataJson);
                        commandReader(action, person);
                        

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer el archivo: " + ex.Message);
            }
        }

        public void CrearUsuarios(BigInteger ClaveP)
        {
            Docs docs = new Docs();
            int index = 0;
            string contra = "";
            string cifrada = "";
            foreach (var usuario in ListaUsuarios)
            {
                string reclutador = usuario.recluiter;
                reclutador = reclutador.Replace(" ", "");
                index = reclutadores.IndexOf(reclutador);
                if (index == -1)
                {
                    reclutadores.Add(reclutador);
                    contra = reclutador+"123-"+ClaveP;
                    UsuarioContra.Add(reclutador + "|" + reclutador + "123");
                    cifrada = docs.Cifrado(contra,"millave", '-');
                    ContraCifrada.Add(reclutador+"|"+cifrada);

                }

            }

            using (StreamWriter writer = new StreamWriter(@"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\Lab4y5\Usuarios.txt"))
            {
                writer.WriteLine("Usuario|Contrasena");
                foreach (string usuario in UsuarioContra)
                {
                    writer.WriteLine(usuario);
                }
            }

            using (StreamWriter writer = new StreamWriter(@"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\Lab4y5\UsuariosC.txt"))
            {
                writer.WriteLine("Usuario|Contrasena");
                foreach (string usuario in ContraCifrada)
                {
                    writer.WriteLine(usuario);
                }
            }


        }


        public void commandReader(string action, Persona person)
        {

            if (action == "INSERT")
            {
                try
                {
                    ListaUsuarios.Add(person);
                    //Console.WriteLine("Insert hecho");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("El error es " + ex);
                    throw;
                }


            }
            else if (action == "DELETE")
            {
                try
                {
                    ListaUsuarios.Remove(person);
                    //Console.WriteLine("Delete hecho");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("El error es " + ex);
                    throw;
                }

            }
            else if (action == "PATCH")
            {
                try
                {

                    //Console.WriteLine("Patch hecho");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("El error es " + ex);
                    throw;
                }

            }
        }

        public void VerificarLogin(string usuario, string contrasena)
        {
            List<string> lineas = new List<string>();
            string decifrada = "";
            string contra = "";
            Docs docs = new Docs();
            bool verif = false;

            using (StreamReader reader = new StreamReader(@"C:\Users\Diego\OneDrive\Documentos\Diego\Universidad\5. Cuarto Ciclo\Estructuras II\Lab4y5\UsuariosC.txt"))
            {
                string linea;
                while ((linea = reader.ReadLine()) != null)
                {
                    
                    lineas.Add(linea);
                }

                for (int i = 0; i < lineas.Count(); i++)
                {
                    string[] separados = lineas[i].Split('|');
                    if (usuario == separados[0])
                    {
                        contra = separados[1];
                        contra = docs.Descifra2(contra, "millave");
                        string[] final = contra.Split("-");
                        if (final[0] == contrasena) 
                        {
                            ImprimirUsuarios(usuario);
                        }
                        else{
                            Console.WriteLine("Usuario y/o contraseña incorrecto");
                        }
                    }
                }
            }

        }

        public void ImprimirUsuarios(string usuario)
        {
            int i = 0;
            Console.WriteLine("\nBienvenid@ " + usuario+ "\n");
            Console.WriteLine("Tus aplicantes son:\n");
            while (i<ListaUsuarios.Count()) {
                string actual = ListaUsuarios[i].recluiter;
                actual = actual.Replace(" ", "");
                
                if (usuario == actual)
                {
                    string companias = String.Join(", ", ListaUsuarios[i].companies);
                    Console.WriteLine("Nombre:" + ListaUsuarios[i].name+" DPI:"+ ListaUsuarios[i].DPI+" Fecha de nacimiento:"+ ListaUsuarios[i].datebirth+
                        " dirección:"+ ListaUsuarios[i].address+" Compañias:" + companias+" Reclutador:"+ ListaUsuarios[i].recluiter+"\n");
                }
                i++;
            }
        }


    }
}

