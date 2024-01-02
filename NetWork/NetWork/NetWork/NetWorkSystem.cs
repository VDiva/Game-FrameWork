
using System;
using System.Net;
using System.Net.Sockets;
using NetWork.Enum;
using NetWork.NetWork.Message;

namespace NetWork
{
    public class NetWorkSystem
    {

        #region ����
        /// <summary>
        /// ���ӵ�socket
        /// </summary>
        private System.Net.Sockets.Socket _socket;

        /// <summary>
        /// ���ݴ������Ĵ�С
        /// </summary>
        private int _count;
       
        /// <summary>
        /// �ͻ��������׽���ֻ��tcpʹ��
        /// </summary>
        private SocketAsyncEventArgs _accept;

        /// <summary>
        /// �����������ɹ��ص�
        /// </summary>
        public Action OpenServer;

        /// <summary>
        /// ��Ϣ���ܳɹ��ص�ֻ��tcpʹ��
        /// </summary>
        public Action<byte[],Client, SocketAsyncEventArgs> ReceiveSuccessAction;

        /// <summary>
        /// �ͻ������ӳɹ��ص�֮��tcpʹ��
        /// </summary>
        public Action<object, SocketAsyncEventArgs> acceptAction;

        /// <summary>
        /// �ͻ��˷���idֻ��tcpʹ��
        /// </summary>
        private int index=0;

        #endregion


        #region �ͻ���Tcp��ʽ����
        public Client NetAsClientTcp(string ip,int port,int count)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipEndPoint);
            var client = new Client(_socket, count);
            return client;
        }
        #endregion


        #region �ͻ���Udp����
        public Client NetAsClientUdp(string ip, int port, int count)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(ipEndPoint);
            _socket.Connect(ipEndPoint);
            var client = new Client(_socket, count);
            return client;
        }
        #endregion


        #region Tcp����������
        public void NetAsServerTcp(string ip,int port,int maxAccept,int count)
        {
            
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ipEndPoint);
            _socket.Listen(maxAccept);
            _accept = new SocketAsyncEventArgs();
            _accept.Completed += OnAcceptCompleted;
            _count=count;
            ReceiveSuccessAction += Parse;
            WaitAccept();
            OpenServer?.Invoke();
        }
        #endregion


        #region Udp����������
        /// <returns></returns>
        public Client NetAsServerUdp(string ip, int port, int count)
        {

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(ipEndPoint);
            Client client = new Client(_socket, count);
            client.ReceiveSuccessAction += ReceiveSuccessAction;
            ReceiveSuccessAction += Parse;
            OpenServer?.Invoke();
            return client;
        }
        #endregion


        #region �������ȴ�����
        /// <summary>
        /// �ȴ��ͻ�������
        /// </summary>
        private void WaitAccept()
        {
            bool success=_socket.AcceptAsync(_accept);
            if (!success)
            {
                SuccessConnect();
            }
        }
       
        /// <summary>
        /// �ͻ������ӳɹ�
        /// </summary>
        void  SuccessConnect()
        {
            Client cli=new Client(_accept.AcceptSocket, _count) { ID = index };
            cli.ReceiveSuccessAction += ReceiveSuccessAction;
            _accept.AcceptSocket = null;
            acceptAction?.Invoke(_accept.AcceptSocket,_accept);
            index += 1;
            WaitAccept();
        }
        
        /// <summary>
        /// �ͻ������ӳɹ��׽��ֻص�
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void OnAcceptCompleted(object obj,SocketAsyncEventArgs args)
        {
            SuccessConnect();
        }
        #endregion


        #region ������Ϣ

        private void Parse(byte[] bytes, Client client, SocketAsyncEventArgs args)
        {
            if(Tool.Tool.DeSerialize(bytes, out GameData.Data data))
            {
                NetWork.Data.QueueData queueData=MessageData.QueueData.DeQueue();
                queueData.client = client;
                queueData.data = data;
                MessageProcessing.Instance.AddData(queueData);
            }

        }

        #endregion

    }
}