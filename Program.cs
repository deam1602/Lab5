using System;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Collections;
using System.Xml.Linq;
using RSA;

namespace RsaEncryptionSample
{
    class Program
    {
        //Variables globales
        List<BigInteger> ListaDivisores = new List<BigInteger>();
        Docs docs = new Docs();
        
         

        static void Main(string[] args)
        {
            Login logg = new Login();
            //Globales
            RSAA rsa = new RSAA();
            BigInteger p = 0;
            BigInteger q = 0;
            BigInteger mensajeBigInt = 1222;
            BigInteger CPublica = 0;
            BigInteger CPrivada = 0;
            //obtener numero primo para variable Q-----------------------------------------------------------------------
            BigIntegerPrimeTest BIPT = new BigIntegerPrimeTest();
            RandomBigIntegerGenerator RBI = new RandomBigIntegerGenerator();
            int bitlen = 15; //cantidad de bits 
            BigInteger RandomNumber = RBI.NextBigInteger(bitlen);
            while (BIPT.IsProbablePrime(RandomNumber, 100) != true) { RandomNumber = RBI.NextBigInteger(bitlen); }
            if (BIPT.IsProbablePrime(RandomNumber, 100) == true) { q = RandomNumber; }
            //------------------------------------------------------------------------------------------------------------
            //obtener numero primo para variable P
            BigIntegerPrimeTest BIPT2 = new BigIntegerPrimeTest();
            RandomBigIntegerGenerator RBI2 = new RandomBigIntegerGenerator();
            int bitlen2 = 15; //cantidad de bits 
            BigInteger RandomNumber2 = RBI2.NextBigInteger(bitlen2);
            while (BIPT2.IsProbablePrime(RandomNumber2, 100) != true) { RandomNumber2 = RBI2.NextBigInteger(bitlen2); }
            if (BIPT2.IsProbablePrime(RandomNumber2, 100) == true) { p = RandomNumber2; }
            //---------------------------------------------------------------------------------------------------------------

            BigInteger ClaveP = rsa.ClavePublica(p, q, mensajeBigInt);

            try
            {
                Console.WriteLine("¿Qué desea hacer? \n 1.MOSTRAR CONVERSACIONES CIFRADAS\n 2.MOSTRAR CONVERSACIONES DESCRIFADAS\n 3.INGRESAR RECLUTADOR");
                string opcion = Console.ReadLine().Trim();
                while (opcion != "0")
                {
                    switch (opcion)
                    {
                        case "1":
                            try
                            {
                                Console.WriteLine("Escriba el DPI de la persona que desea buscar: ");
                                string dpi_buscar = Console.ReadLine().Trim();
                                long dpi_convertido = Convert.ToInt64(dpi_buscar);

                                Console.WriteLine("Se esta haciendo su busqueda...\n");
                                
                                Docs.BuscarArchivoCifrar(dpi_convertido, ClaveP);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }

                            Console.WriteLine("\n¿Qué desea hacer? \n 1.MOSTRAR CONVERSACIONES CIFRADAS\n 2.MOSTRAR CONVERSACIONES DESCRIFADAS\n 3.INGRESAR RECLUTADOR\"");
                            opcion = Console.ReadLine().Trim();
                            break;
                        case "2":
                            ConversacionesDescifradas();
                            Console.WriteLine("\n¿Qué desea hacer? \n 1.MOSTRAR CONVERSACIONES CIFRADAS\n 2.MOSTRAR CONVERSACIONES DESCRIFADAS\n 3.INGRESAR RECLUTADOR\"");
                            opcion = Console.ReadLine().Trim();
                            break;
                        case "3":
                            logg.leerArchivo();
                            logg.CrearUsuarios(ClaveP);
                            Console.WriteLine("Escriba su nombre de usuario:");
                            string usuario = Console.ReadLine().Trim();
                            Console.WriteLine("Escriba su contraseña:");
                            string contra = Console.ReadLine().Trim();
                            logg.VerificarLogin(usuario, contra);
                            Console.WriteLine("\n¿Qué desea hacer? \n 1.MOSTRAR CONVERSACIONES CIFRADAS\n 2.MOSTRAR CONVERSACIONES DESCRIFADAS\n 3.INGRESAR RECLUTADOR\"");
                            opcion = Console.ReadLine().Trim();
                            break;
                        default:
                            Console.WriteLine("Ninguna opcion elegida");
                            break;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }




        }

        //obtener Hash
        public static BigInteger CalcularHashSHA256(string mensaje)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(mensaje);//Convertirmos mensaje en bytes
                byte[] hashBytes = sha256.ComputeHash(bytes); //Obtenemos el hash de lo bytes del mensaje
                BigInteger mensajeBigInt = new BigInteger(hashBytes); //Convertimos el arreglo de bytes en un BigInteger

                return mensajeBigInt;
            }
        }


        public static void ConversacionesDescifradas()
        {
            try
            {
                Console.WriteLine("Escriba el DPI de la persona que desea buscar: ");
                string dpi_buscar = Console.ReadLine().Trim();
                long dpi_convertido = Convert.ToInt64(dpi_buscar);

                Console.WriteLine("Se esta haciendo su busqueda...\n");
                Docs.BuscarArchivoDescifrar(dpi_convertido);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

    }

    public class RSAA
    {
        List<BigInteger> ListaDivisores = new List<BigInteger>();

        public BigInteger ClavePublica(BigInteger p, BigInteger q, BigInteger mensaje)
        {
            BigInteger clave = 0;
            BigInteger z = Z(p, q);
            ObtenerDivisioresZ(z);
            BigInteger kk = K(z, ListaDivisores);
            //clave = BigInteger.Pow(mensaje, k) % N(p,q);
            clave = BigInteger.Pow(1222, (int)kk) % N(p, q);

            return clave;
        }

        public BigInteger ClavePrivada(BigInteger p, BigInteger q, BigInteger clave)
        {
            BigInteger mensaje = 0;
            BigInteger z = Z(p, q);
            ObtenerDivisioresZ(z);
            BigInteger k = K(z, ListaDivisores);
            BigInteger jj = J(k,z);
            int j = (int)jj;
            mensaje = BigInteger.Pow(clave, j) % N(p, q);

            return mensaje;
        }

        
        public BigInteger N(BigInteger p, BigInteger q)
        {

            BigInteger N = BigInteger.Multiply(p, q);
            return N;
        }

        public BigInteger Z(BigInteger p, BigInteger q)
        {
            //realiza la operación = (p - 1) * (q - 1); = se realiza de esta forma porque son BigInteger
            BigInteger Z = BigInteger.Multiply(BigInteger.Subtract(p,1), BigInteger.Subtract(q, 1));
            return Z;
        }


        public List<BigInteger> ObtenerDivisioresZ(BigInteger Z)
        {
            //List<int> ListaDivisores = new List<int>();
            BigInteger Cociente = 0, Divisor = 1, Dividendo = Z;
            while (Divisor <= Dividendo)
            {
                if (Dividendo % Divisor == 0)
                {
                    Cociente = Dividendo / Divisor;
                    ListaDivisores.Add(Divisor);
                    Divisor++;
                }
                else
                {
                    Divisor++;
                }
            }
            return ListaDivisores;
        }

        public BigInteger K(BigInteger z, List<BigInteger> DivisoresZ)
        {
            BigInteger k = 0;
            BigInteger MCD = 0;
            int cantDiv = DivisoresZ.Count();
            //obtener numero primo para variable K
            BigIntegerPrimeTest BIPTK = new BigIntegerPrimeTest();
            RandomBigIntegerGenerator RBIK = new RandomBigIntegerGenerator();
            int bitlenk = 10; //cantidad de bits 
            BigInteger RandomNumber2 = RBIK.NextBigInteger(bitlenk);
            while (BIPTK.IsProbablePrime(RandomNumber2, 100) != true) { RandomNumber2 = RBIK.NextBigInteger(bitlenk); }
            if (BIPTK.IsProbablePrime(RandomNumber2, 100) == true) { k = RandomNumber2; }
            //---------------------------------------------------------------------------------------------------------------

            for (int i = 0; i < cantDiv; i++)
            {
                if (DivisoresZ[i] == k) //Si ka es igual a uno de los divisores el MCD es mayor a 1
                {
                    k = RBIK.NextBigInteger(bitlenk);
                }
                else if (DivisoresZ[i] > k && DivisoresZ[i] != k) //Si el divisor de Z es mayor que K significa que ya pasó el maximo divisor de K
                {
                    MCD = 1;// y si este es diferente a K significa que el MCD es 1
                }
                i++;
            }
            

            return k;
        }

        public BigInteger J(BigInteger K, BigInteger Z)
        {
            BigInteger J = 0;
            BigInteger mod = 0;
            BigInteger i = 0;
            double aprox = 0;

            while (aprox != 1)
            {
                mod = BigInteger.Multiply(K,i)%Z;
                aprox = (double)mod;
                J = i;
                i++;
            }
            return J;
        }
    }



    class RandomBigIntegerGenerator
    {
        public BigInteger NextBigInteger(int bitLength)
        {
            if (bitLength < 1) return BigInteger.Zero;

            int bytes = bitLength / 8;
            int bits = bitLength % 8;

            // Generates enough random bytes to cover our bits.
            Random rnd = new Random();
            byte[] bs = new byte[bytes + 1];
            rnd.NextBytes(bs);

            // Mask out the unnecessary bits.
            byte mask = (byte)(0xFF >> (8 - bits));
            bs[bs.Length - 1] &= mask;

            return new BigInteger(bs);
        }

        // Random Integer Generator within the given range
        public BigInteger RandomBigInteger(BigInteger start, BigInteger end)
        {
            if (start == end) return start;

            BigInteger res = end;

            // Swap start and end if given in reverse order.
            if (start > end)
            {
                end = start;
                start = res;
                res = end - start;
            }
            else
                // The distance between start and end to generate a random BigIntger between 0 and (end-start) (non-inclusive).
                res -= start;

            byte[] bs = res.ToByteArray();

            // Count the number of bits necessary for res.
            int bits = 8;
            byte mask = 0x7F;
            while ((bs[bs.Length - 1] & mask) == bs[bs.Length - 1])
            {
                bits--;
                mask >>= 1;
            }
            bits += 8 * bs.Length;

            // Generate a random BigInteger that is the first power of 2 larger than res, 
            // then scale the range down to the size of res,
            // finally add start back on to shift back to the desired range and return.
            return ((NextBigInteger(bits + 1) * res) / BigInteger.Pow(2, bits + 1)) + start;
        }
    }


    // Miller-Rabin primality test as an extension method on the BigInteger type.
    // Based on the Ruby implementation on this page.
    class BigIntegerPrimeTest
    {
        public bool IsProbablePrime(BigInteger source, int certainty)
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    // This may raise an exception in Mono 2.10.8 and earlier.
                    // http://bugzilla.xamarin.com/show_bug.cgi?id=2761
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }
    }
}
