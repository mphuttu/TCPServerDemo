using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Jokeri: Lisää timestamp Person:iin
// Niin että 
// Muuta tietoja palvelimella ja palauta muutetut tidot clienttiin
// TCP Socket Listener ja client, muutetaan softaa niin että me sekä 
// vastaanotamme että lähetämme JSON:ia.

namespace TCPServerDemo
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    class Program
    {
        // Incoming data from the client.  
        public static string data = null;
        public static string result = null;

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        // data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        data += Encoding.Default.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            result = data.Remove(data.Length - 5);
                            break;                            
                        }
                    }

                    // Show the data on the console.  
                    //Console.WriteLine("Text received : {0}", data);
                    Console.WriteLine("Text received : {0}", result);

                    List<Person> mylist = JsonConvert.DeserializeObject<List<Person>> (result);
                    //Console.WriteLine(mylist.First<Person>().FirstName);
                    //Console.WriteLine(mylist.First<Person>().LastName);

                    foreach ( Person iter in mylist)
                    {
                        Console.WriteLine(iter.FirstName + " " + iter.LastName);
                        iter.FirstName += " ABC";
                        iter.LastName += " ÅÄÖ";
                        
                    }

                    // Echo the data back to the client.  
                    // byte[] msg = Encoding.ASCII.GetBytes(data);
                    // Timestamp
                    String timeStamp = GetTimestamp(DateTime.Now);
                    data += "\nSuoritettu aikana " + timeStamp;
                    byte[] msg = Encoding.Default.GetBytes(data);
                    

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private static string GetTimestamp(DateTime now)
        {
            // throw new NotImplementedException();
            return now.ToString("yyyy-MM-dd HH:mm:ss ffff");
        }

        static int Main(string[] args)
            {
                StartListening();
                return 0;
            } // end of Main
    } // end of Program
} // end of Namespace
