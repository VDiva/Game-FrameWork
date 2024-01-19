﻿
using NetWork.Type;
using Riptide;

namespace NetWork
{
    public class Room
    {
        public int roomId;
        public string roomName;

        public int maxCount;
        //private List<Connection> players;
        private List<Message> messages;

        private Dictionary<ushort, Connection> players;

        public Room(int roomId,string roomName,int maxCount)
        {
            this.roomId = roomId;
            this.roomName = roomName;
            this.maxCount = maxCount;
            
            players = new Dictionary<ushort, Connection>();
            messages = new List<Message>();
           
        }

        public Room()
        {
            players = new Dictionary<ushort, Connection>();
            messages = new List<Message>();
        }

        public void Init(int roomId, string roomName, int maxCount)
        {
            this.roomId = roomId;
            this.roomName = roomName;
            this.maxCount = maxCount;

            ReleaseMessage();

            messages.Clear();
            players.Clear();

        }
        public void Init( string roomName, int maxCount)
        {
            this.roomName = roomName;
            this.maxCount = maxCount;
            ReleaseMessage();
            messages.Clear();
            players.Clear();
        }

        public bool Join(ushort id,Connection connection)
        {
            if (!players.ContainsKey(id))
            {
                if (players.Count < maxCount)
                {
                    Console.WriteLine(id + ":加入了房间:" + roomId);
                    players.Add(id, connection);

                    SendHistoryInformation(connection, () =>
                    {
                        Message msg = NetWorkSystem.CreateMessage(MessageSendMode.Reliable, ServerToClientMessageType.PlayerJoinRoom);
                        msg.AddUShort(id);
                        msg.AddInt(roomId);
                        SendAll(msg);
                    });
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public void Left(ushort id)
        {
            if (players.ContainsKey(id))
            {
                Message msg = NetWorkSystem.CreateMessage(MessageSendMode.Reliable, ServerToClientMessageType.PlayerLeftRoom);
                msg.AddUShort(id);
                SendAll(msg);

                players.Remove(id);
                Console.WriteLine(id + ":离开了房间:" + roomId);
                if(players.Count == 0)
                {
                    Console.WriteLine(roomId+"房间空 回收房间");
                    RoomSystem.EnQueue(this);
                }
            }
        }


        public void Transform(Message message)
        {
           SendAll(message,false);
        }



        private void SendHistoryInformation(Connection connection,Action action=null)
        {
            
            foreach (Message message in messages)
            {
                connection.Send(message,false);
            }

            action?.Invoke();
        }

        public void SendAll(Message message,bool isAdd=true)
        {
            if(isAdd) AddMessage(message);
            foreach (var player in players)
            {
                player.Value.Send(message,false);
            }

            
        }

        public void SendOther(ushort id,Message message, bool isAdd = true)
        {
            if (isAdd) AddMessage(message);
            foreach (var player in players)
            {
                if (player.Value.Id != id)
                {
                    player.Value.Send(message, false);
                }
            }
        }

        public void SendSelf(ushort id, Message message, bool isAdd = true)
        {
            if (isAdd) AddMessage(message);
            foreach (var player in players)
            {
                if (player.Value.Id == id)
                {
                    player.Value.Send(message, false);
                }
            }
            
        }

        private void AddMessage(Message message)
        {
            messages.Add(message);
        }


        private void ReleaseMessage()
        {
            for(var i=0; i<messages.Count; i++)
            {
                messages[i].Release();
            }
        }

    }
}