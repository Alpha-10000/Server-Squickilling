using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
namespace Serveur
{
    enum PacketTypes
    {
        LOGIN,
        NEWPERSO,
        POSITIONX,
        POSITIONY,
        SCORE,
        BONUS,
        HEALTH,
        PROJ,
        IA
    }

    class Program
    {
        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("squickilling");
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.Port = 14242;
            config.MaximumConnections = 200;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            NetServer server = new NetServer(config);
            NetOutgoingMessage outmsg = server.CreateMessage();
            server.Start();
            Console.WriteLine("Serveur créé, j'attends mon café");

            List<Game> myGames = new List<Game>();

            Dictionary<NetConnection, int> clients = new Dictionary<NetConnection, int>();
            List<NetConnection> AllClients = new List<NetConnection>();

            while (true)
            {

                NetIncomingMessage inc;
                if ((inc = server.ReadMessage()) != null)
                {
                    byte truc = inc.ReadByte();
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            Console.WriteLine("Connexion approuvé");

                            string id_player = inc.ReadString();
                            bool check = true;
                            for (int i = 0; i < myGames.Count; i++)
                            {
                                if (myGames[i].id_player == id_player)
                                {
                                    myGames[i].Create(inc, outmsg);
                                    check = false;
                                }
                            }

                                    if(check)
                                    {
                                        Game game = new Game(server, id_player);
                                        game.Create(inc, outmsg);
                                        myGames.Add(game);
                                    }


                            break;

                        case NetIncomingMessageType.Data:
                            for (int i = 0; i < myGames.Count; i++)
                                myGames[i].TypeData(inc, truc, outmsg);      

                            break;
                            case NetIncomingMessageType.StatusChanged:
                            for (int i = 0; i < myGames.Count; i++)
                                myGames[i].StatusChanged(inc);
        
                            break;
                        default:
                            Console.WriteLine("Je reçois autre chose");
                            break;

                    }
                }
            }
            }
    }
}
