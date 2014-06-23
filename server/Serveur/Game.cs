using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace Serveur
{
    class Game
    {
        private NetServer server;
        public int index { get; private set; }
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
            Thread.Sleep(500);
            clients.Add(senderConnection, index);
            AllClients.Add(senderConnection);
            index++;
            for (int i = 0; i <= index; i++)
                for (int j = 0; j <= index; j++)
                {
                    outmsg = server.CreateMessage();
                    outmsg.Write((byte)PacketTypes.NEWPERSO);
                    outmsg.Write(i);
                    outmsg.Write(index + 1);
                    server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                }
        }

        public void TypeData(NetIncomingMessage inc, byte truc, NetOutgoingMessage outmsg)
        {
            if (truc == (byte)PacketTypes.POSITIONX)
            {
                int whichPersoIndex = inc.ReadInt32();
                float newPosX = inc.ReadFloat();
                for (int j = 0; j <= index; j++)
                {
                    if (j != whichPersoIndex)
                    {
                        try // It has to be!!! Because maybe he doesn't know if the perso has disconnected or not
                        {
                            outmsg = server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.POSITIONX);
                            outmsg.Write(whichPersoIndex);
                            outmsg.Write(newPosX);
                            server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            if (truc == (byte)PacketTypes.POSITIONY)
            {
                int whichPersoIndex = inc.ReadInt32();
                float newPosY = inc.ReadFloat();
                for (int j = 0; j <= index; j++)
                {
                    if (j != whichPersoIndex)
                    {
                        try // It has to be!!! Because maybe he doesn't know if the perso has disconnected or not
                        {
                            outmsg = server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.POSITIONY);
                            outmsg.Write(whichPersoIndex);
                            outmsg.Write(newPosY);
                            server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                        }
                        catch
                        {

                        }
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

        public void StatusChanged(NetIncomingMessage inc, NetOutgoingMessage outmsg)
        {
            Console.WriteLine("status a changé ");
            for (int i = 0; i < AllClients.Count; i++)
                if (AllClients[i].Status == NetConnectionStatus.Disconnected || AllClients[i].Status == NetConnectionStatus.Disconnecting)
                {
                    index--;
                    AllClients.Remove(AllClients[i]);
                    for (int j = 0; j < AllClients.Count; j++)
                    {
                        outmsg = server.CreateMessage();
                        outmsg.Write((byte)PacketTypes.PERSOLEAVE);
                        outmsg.Write(i);
                        server.SendMessage(outmsg, AllClients[j], NetDeliveryMethod.ReliableOrdered);
                    }
                    Console.WriteLine("the player id " + i + " has disconnected");
                }
        }
                        
    }
}
