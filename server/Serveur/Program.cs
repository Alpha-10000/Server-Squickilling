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
            Console.WriteLine("Serveur créé, en attente de connexions");

            int index = 0;
            Dictionary<NetConnection, int> clients = new Dictionary<NetConnection, int>();

            while (true)
            {
                NetIncomingMessage inc;
                if ((inc = server.ReadMessage()) != null)
                {
                    byte truc = inc.ReadByte();
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            if (truc == (byte)PacketTypes.LOGIN)
                            {
                                //inc.ReadString();
                                NetConnection senderConnection = inc.SenderConnection;
                                inc.SenderConnection.Approve();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {
                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.NEWPERSO);
                                    outmsg.Write(index);
                                    server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered);
                                }
                                clients.Add(senderConnection, index);
                                index++;
                                Console.WriteLine("accepté" + index);
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            Console.WriteLine("truc qui vient");
                            if (truc == (byte)PacketTypes.POSITIONX)
                            {
                                NetConnection senderConnection = inc.SenderConnection;
                                float pos = inc.ReadFloat();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {
                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.POSITIONX);
                                    //envoi de l'index du client à modifier.
                                    outmsg.Write(clients[senderConnection]);
                                    outmsg.Write(pos);
                                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("posx");
                                }
                            }
                            if (truc == (byte)PacketTypes.POSITIONY)
                            {
                                NetConnection senderConnection = inc.SenderConnection;
                                float pos = inc.ReadFloat();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {
                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.POSITIONY);
                                    //envoi de l'index du client à modifier.
                                    outmsg.Write(clients[senderConnection]);
                                    outmsg.Write(pos);
                                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("posy");
                                }
                            }
                            if (truc == (byte)PacketTypes.SCORE)
                            {
                                NetConnection senderConnection = inc.SenderConnection;
                                int score = inc.ReadInt32();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {

                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.SCORE);
                                    //envoi de l'index du client à modifier.
                                    outmsg.Write(clients[senderConnection]);
                                    outmsg.Write(score);
                                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("score");
                                }
                            }
                            if (truc == (byte)PacketTypes.BONUS)
                            {
                                Console.WriteLine("bonus");
                                NetConnection senderConnection = inc.SenderConnection;
                                int bonus = inc.ReadInt32();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {

                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.BONUS);
                                    //envoi de l'index du client à modifier.
                                    outmsg.Write(clients[senderConnection]);
                                    outmsg.Write(bonus);
                                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);

                                }
                            }
                            if (truc == (byte)PacketTypes.HEALTH)
                            {
                                NetConnection senderConnection = inc.SenderConnection;
                                int health = inc.ReadInt32();
                                for (int i = 0; i < server.ConnectionsCount; i++)
                                {

                                    outmsg = server.CreateMessage();
                                    outmsg.Write((byte)PacketTypes.HEALTH);
                                    //envoi de l'index du client à modifier.
                                    outmsg.Write(clients[senderConnection]);
                                    outmsg.Write(health);
                                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                                }
                                Console.WriteLine("santé");
                            }

                            break;
                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine(inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status);
                            if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                // Servira à retirer le perso de la partie s'il est déconnecté
                                //foreach (Perso p in in persos)
                                //{
                                //    if (p.Connection == inc.SenderConnection)
                                //    {
                                //        persos.remove(p);
                                //        break;
                                //    }
                                //}
                            }
                            break;
                        default:
                            Console.WriteLine("osef" + Convert.ToString(truc));
                            break;

                    }
                }

            }
        }
    }
}
