﻿using NetWork.Tool;
using NetWork.Type;
using Riptide;


namespace NetWork
{
    public static class RoomSystem
    {

        private static Dictionary<int, Room> rooms;
        private static Dictionary<ushort, Room> playerIdGetRoom;
        private static ObjectPool<Room> objectPoolRoom;
        private static int index;

        static RoomSystem()
        {
            rooms = new Dictionary<int, Room>();
            playerIdGetRoom = new Dictionary<ushort, Room>();
            objectPoolRoom = new ObjectPool<Room>();
        }

        [MessageHandler((ushort)ClientToServerMessageType.JoinRoom)]
        private static void JoinRoom(ushort id,Message message)
        {
            var roomId = message.GetInt();
            if (rooms.ContainsKey(roomId))
            {
                if (!rooms[roomId].Join(id,NetWorkSystem.GetClient(id)))
                {
                    SendError(id, "房间以满");
                }
                else
                {
                    //SendJoinRoomSuccess(id, rooms[roomId]);
                }
            }
            else
            {
                SendError(id, "房间不存在");
            }
        }

        [MessageHandler((ushort)ClientToServerMessageType.CreateRoom)]
        private static void CreateRoom(ushort id, Message message)
        {
            var roomName = message.GetString();
            var roomCount = message.GetInt();
            Room room = objectPoolRoom.DeQueue(room => { room.Init(index, roomName, roomCount); index += 1; }, room => { room.Init(roomName, roomCount); });
            //Room room = new Room(index, roomName, roomCount);
            if (playerIdGetRoom.TryAdd(id, room))
            {
                rooms.TryAdd(room.roomId, room);
                room.Join(id, NetWorkSystem.GetClient(id));
                //SendJoinRoomSuccess(id, room);
            }
            else
            {
                Console.WriteLine("玩家以再房间中创建失败");
                EnQueue(room);
            }
            
            
        }

        [MessageHandler((ushort)ClientToServerMessageType.MatchingRoom)]
        private static void MatchingRoom(ushort id, Message message)
        {
            var client = NetWorkSystem.GetClient(id);
            var roomName = message.GetString();
            var roomCount = message.GetInt();
            if (rooms.Count>0)
            {
                if (!playerIdGetRoom.ContainsKey(id))
                {

                    foreach(var ro in rooms)
                    {
                        if (ro.Value.Join(id, client))
                        {
                            playerIdGetRoom.TryAdd(id, ro.Value);
                            //SendJoinRoomSuccess((ushort)id, ro.Value);
                            return;
                        }
                    }


                    Room room = objectPoolRoom.DeQueue(room => { room.Init(index, roomName, roomCount); index += 1; }, room => { room.Init(roomName, roomCount); });
                    
                    //Room room = new Room(index, message.GetString(), message.GetInt());

                    if (playerIdGetRoom.TryAdd(id, room))
                    {
                        rooms.TryAdd(room.roomId, room);
                        room.Join(id, NetWorkSystem.GetClient(id));
                        //SendJoinRoomSuccess(id, room);
                    }
                    else
                    {
                        EnQueue(room);
                    }
                }
                else
                {
                    Console.WriteLine("玩家以存在房间中。。。");
                } 
            }
            else
            {
                Room room = objectPoolRoom.DeQueue(room => { room.Init(index, roomName, roomCount); index += 1; }, room => { room.Init(roomName, roomCount); });
                //Room room = new Room(index, message.GetString(), message.GetInt());
                if (playerIdGetRoom.TryAdd(id, room))
                {
                    rooms.TryAdd(room.roomId, room);
                    room.Join(id, NetWorkSystem.GetClient(id));
                    //SendJoinRoomSuccess(id, room);
                }
                else
                {
                    EnQueue(room);
                }
            }
        }



        [MessageHandler((ushort)ClientToServerMessageType.Transform)]
        private static void TransfromAll(ushort id,Message message)
        {
            try
            {
                if (playerIdGetRoom.TryGetValue(id, out var room))
                {
                    //var tick = NetWorkSystem.GetTick();
                    //Console.WriteLine(tick.ToString());
                    Message msg = NetWorkSystem.CreateMessage(MessageSendMode.Unreliable, ServerToClientMessageType.Transform);

                    var objId = message.GetUShort();

                    var pos = message.GetVector3();
                    //var x = message.GetVector3();
                    //var y = message.GetFloat();
                    //var z = message.GetFloat();


                    var rot = message.GetVector3();
                    //var rx = message.GetFloat();
                    //var ry = message.GetFloat();
                    //var rz = message.GetFloat();


                    msg.AddUShort(objId);
                    msg.AddVector3(pos);
                    //msg.AddFloat(x);
                    //msg.AddFloat(y);
                    //msg.AddFloat(z);

                    msg.AddVector3(rot);
                    //msg.AddFloat(rx);
                    //msg.AddFloat(ry);
                    //msg.AddFloat(rz);
                    
                    room.Transform(id,msg,objId,pos,rot);
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [MessageHandler((ushort)ClientToServerMessageType.LeftRoom)]
        private static void LeftRoom(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.Left(id);
                playerIdGetRoom.Remove(id);
            }
        }


        [MessageHandler((ushort)ClientToServerMessageType.Instantiate)]
        private static void Instantiate(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.Instantiate(id,message);
            }
        }


        [MessageHandler((ushort)ClientToServerMessageType.Rpc)]
        private static void Rpc(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.Rpc(id,message);
            }
        }

        [MessageHandler((ushort)ClientToServerMessageType.GetId)]
        private static void GetId(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.Rpc(id, message);
            }
        }


        [MessageHandler((ushort)ClientToServerMessageType.Destroy)]
        private static void Destroy(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                var objId = message.GetUShort();
                room.Destroy(objId);
            }
        }

        [MessageHandler((ushort)ClientToServerMessageType.ReLink)]
        private static void ReLink(ushort id, Message message)
        {
            var oldId=message.GetUShort();
            if (playerIdGetRoom.TryGetValue(oldId, out var room))
            {
                room.ReLink(id, oldId);
                playerIdGetRoom.Remove(oldId);
                playerIdGetRoom.Add(id, room);
            }
        }

        [MessageHandler((ushort)ClientToServerMessageType.CloseGame)]
        private static void CloseGame(ushort id, Message message)
        {
            if (playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.Left(id);
            }
        }




        private static void SendJoinRoomSuccess(ushort id,Room room)
        {
            //Message msg = NetWorkSystem.CreateMessage(MessageSendMode.Reliable, ServerToClientMessageType.JoinRoomSuccess);
            //msg.AddInt(room.roomId);
            //msg.AddString(room.roomName);
            //NetWorkSystem.Send(msg, id);
        }


        public static void EnQueue(Room room)
        {
            objectPoolRoom.EnQueue(room);
            rooms.Remove(room.roomId);
            Console.WriteLine("房间数量:" + rooms.Count);
        }

        private static void SendError(ushort id,string info)
        {
            NetWorkSystem.SendError(info, id);
        }

        public static void PlayerDisConnect(ushort id)
        {
            if(playerIdGetRoom.TryGetValue(id, out var room))
            {
                room.PlayerDisConnect(id);
            }
        }



    }
}
