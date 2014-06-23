using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Serveur
{
    class Game
    {
        private NetServer server;
        private int index;
        private Dictionary<NetConnection, int> clients = new Dictionary<NetConnection, int>();
        private List<NetConnection> AllClients = new List<NetConnection>();

        public string id_player;

        public Game(NetServer server, string id_player)
        {
            this.server = server;
            index = -1;
            this.id_player = id_player;

        }

        public void Create(NetIncomingMessage inc, NetOutgoingMessage outmsg)
        {
            NetConnection senderConnection = inc.SenderConnection;
            inc.SenderConnection.Approve();
            clients.Add(senderConnection, index);
            AllClients.Add(senderConnection);
            index++;
            for (int i = 0; i <= index; i++)
            {
                for (int j = 0; j <= index; j++)
                {
                    outmsg = server.CreateMessage();
                    outmsg.Write((byte)PacketTypes.NEWPERSO);
                    outmsg.Write(i);
                    outmsg.Write(index + 1);
                    server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                }
                int debug = index + 1;
                Console.WriteLine("nb player :  " + debug);
                if (i != index)
                    Console.WriteLine("ancien joueur id : " + i);
                else
                    Console.WriteLine("Nouveau joueur accepté id : " + i);
            }
        }

        public void TypeData(NetIncomingMessage inc, byte truc, NetOutgoingMessage outmsg)
        {
            NetConnection senderConnection;
            Console.WriteLine("là je reçois de la data (pos, health ,etc)");
            if (truc == (byte)PacketTypes.POSITIONX)
            {
                senderConnection = inc.SenderConnection;
                int whichPersoIndex = inc.ReadInt32();
                float newPosX = inc.ReadFloat();
                for (int j = 0; j <= index; j++)
                {
                    if (j != whichPersoIndex)
                    {
                        outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.POSITIONX);
                        outmsg.Write(whichPersoIndex);
                        outmsg.Write(newPosX);
                        server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
            if (truc == (byte)PacketTypes.POSITIONY)
            {
                senderConnection = inc.SenderConnection;
                int whichPersoIndex = inc.ReadInt32();
                float newPosY = inc.ReadFloat();
                for (int j = 0; j <= index; j++)
                {
                    if (j != whichPersoIndex)
                    {
                        outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.POSITIONY);
                        outmsg.Write(whichPersoIndex);
                        outmsg.Write(newPosY);
                        server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
            /*
            if (truc == (byte)PacketTypes.SCORE)
            {
                senderConnection = inc.SenderConnection;
                int score = inc.ReadInt32();
                for (int i = 0; i < server.ConnectionsCount; i++)
                {

                    outmsg = server.CreateMessage();
                    outmsg.Write((byte)PacketTypes.SCORE);
                    //envoi de l'index du client à modifier.
                    outmsg.Write(clients[senderConnection]);
                    outmsg.Write(score);
                    server.SendMessage(outmsg, server.Connections[i], NetDeliveryMethod.ReliableOrdered);
                }
            }
            if (truc == (byte)PacketTypes.BONUS)
            {
                           
                senderConnection = inc.SenderConnection;
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
                senderConnection = inc.SenderConnection;
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
                  
            }
            */
        }

        public void StatusChanged(NetIncomingMessage inc)
        {
            Console.WriteLine("status a changé ");
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
        }
                        
    }
}
