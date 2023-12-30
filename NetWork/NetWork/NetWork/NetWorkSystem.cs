
using System;
using System.Net;
using System.Net.Sockets;
using NetWork.Enum;
namespace NetWork
{
    public class NetWorkSystem
    {
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
        public Action<byte[],object, SocketAsyncEventArgs> ReceiveSuccessAction;

        /// <summary>
        /// �ͻ������ӳɹ��ص�֮��tcpʹ��
        /// </summary>
        public Action<object, SocketAsyncEventArgs> acceptAction;

        /// <summary>
        /// �ͻ��˷���idֻ��tcpʹ��
        /// </summary>
        private int index=0;


        /// <summary>
        ///ʹ��Tcp��ʽ����
        /// </summary>
        /// <param name="ip">��ַ</param>
        /// <param name="port">�˿���</param>
        /// <param name="count">������������С</param>
        /// <returns></returns>
        public Client NetAsClientTcp(string ip,int port,int count)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipEndPoint);
            var client = new Client(_socket, count);
            return client;
        }


        /// <summary>
        /// ʹ��udp��ʽ����
        /// </summary>
        /// <param name="ip">��ַ</param>
        /// <param name="port">�˿���</param>
        /// <param name="count">������������С</param>
        /// <returns></returns>
        public Client NetAsClientUdp(string ip, int port, int count)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(ipEndPoint);
            _socket.Connect(ipEndPoint);
            var client = new Client(_socket, count);
            return client;
        }


        /// <summary>
        /// ʹ��Tcp��ʽ����һ��������
        /// </summary>
        /// <param name="ip">��ַ</param>
        /// <param name="port">�˿�</param>
        /// <param name="maxAccept">�����������</param>
        /// <param name="count">������������С</param>
        public void NetAsServerTcp(string ip,int port,int maxAccept,int count)
        {
            
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ipEndPoint);
            _socket.Listen(maxAccept);
            _accept = new SocketAsyncEventArgs();
            _accept.Completed += OnAcceptCompleted;
            _count=count;
            WaitAccept();
            OpenServer?.Invoke();
        }

        /// <summary>
        /// ʹ��udp�ķ�ʽ����һ��������
        /// </summary>
        /// <param name="ip">��ַ</param>
        /// <param name="port">�˿�</param>
        /// <param name="count">������������С</param>
        /// <returns></returns>
        public Client NetAsServerUdp(string ip, int port, int count)
        {

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new System.Net.Sockets.Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(ipEndPoint);
            Client client = new Client(_socket, count);
            OpenServer?.Invoke();
            return client;
        }


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

    }
}