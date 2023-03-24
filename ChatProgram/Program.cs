using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatProgram
{
    class Server
    {
        private Socket _serverSocket; //En socket der vil fungere som server socket
        private List<Socket> _clientSockets = new List<Socket>(); //En liste over klient sockets der er tilsluttet serveren
        private byte[] _buffer = new byte[1024]; //En buffer til at lagre data
      
        public void Start()  //Metode til at starte serveren
        {
            
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Opretter server socket
           
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234)); //Binder server socket til et IP endpoint og en port
           
            _serverSocket.Listen(5); //Lyttet efter forbindelser med en backlog på 5
            
            Console.WriteLine("Server started on port 1234");//Udskriver besked om at serveren er startet på port 1234

            
            while (true)//Kører en uendelig løkke for at acceptere nye forbindelser
            {
                
                Socket clientSocket = _serverSocket.Accept();//Venter på en klientforbindelse
               
                _clientSockets.Add(clientSocket); //Tilføjer klient socket til listen over tilsluttede klienter

                
                Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint.ToString());//Udskriver besked om at en ny klient er tilsluttet serveren

               
                Task.Factory.StartNew(() => { //Starter en ny tråd til at modtage og behandle data fra klienten
                  
                    while (true)  //Kører en uendelig løkke for at modtage data fra klienten
                    {
                       
                        int bytesReceived = clientSocket.Receive(_buffer); //Modtager data fra klienten og returnerer antallet af bytes modtaget
                      
                        if (bytesReceived > 0)  //Hvis der er modtaget data
                        {
                           
                            string message = Encoding.ASCII.GetString(_buffer, 0, bytesReceived); //Konverterer modtaget data til en streng
                            
                            Console.WriteLine(clientSocket.RemoteEndPoint.ToString() + ": " + message);//Udskriver besked om hvilken klient der har sendt dataen og selve dataen

                            foreach (Socket socket in _clientSockets) //Sender modtaget data til alle andre tilsluttede klienter
                           
                            {
                                if (socket != clientSocket) //Sikrer at data ikke sendes tilbage til klienten der sendte den
                                {
                                   
                                    socket.Send(Encoding.ASCII.GetBytes(clientSocket.RemoteEndPoint.ToString() + ": " + message)); //Sender dataen som bytes til modtager socket
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}