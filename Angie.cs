using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace Web
{
  public class Angie
  {
    private Stack<AngieMiddleware> middlewares = new Stack<AngieMiddleware>();
    private AngieEntry entry = null;

    public Angie()
    {
    }


    public Angie use(AngieMiddleware middleware)
    {
      this.middlewares.Push(middleware);

      this.entry = (req, res) =>
      {
        Stack<AngieMiddleware> middlewaresCloned = new Stack<AngieMiddleware>(new Stack<AngieMiddleware>(this.middlewares));

        AngieMiddleware eachMiddleware = null;
        AngieNextMiddleware previousNext = null;
        while (middlewaresCloned.TryPop(out eachMiddleware))
        {
          var currentMiddleware = eachMiddleware;
          var next = previousNext;
          previousNext = () =>
          {
            currentMiddleware(req, res, next);
          };
        }
        previousNext();
      };
      return this;
    }

    public void listen(int port)
    {
      Console.WriteLine(DateTime.Now);
      Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      serverSocket.Bind(new IPEndPoint(0x0, port));
      serverSocket.Listen();
      Socket clientSocket = null;

      while (true)
      {
        if ((clientSocket = serverSocket.Accept()) == null)
        {
          continue;
        }

        AngieThread socketProcessor = (Socket clientSocketInThread) =>
        {
          return () =>
          {
            try
            {
              Byte[] sendByte = new Byte[1024];

              NetworkStream clientStream = new NetworkStream(clientSocketInThread);

              clientStream.Write(Encoding.UTF8.GetBytes("Hello world!"));

              Byte[] receiveByte = new Byte[1024];
              clientStream.Read(new Byte[1024]);
              String httpRequestMessage = ASCIIEncoding.ASCII.GetString(receiveByte);

              this.entry(new AngieRequest(), new AngieResponse());

              // clientStream.Flush();
              clientStream.Close();

              // clientSocketInThread.Send(Encoding.ASCII.GetBytes("Hello world!"));
              // Byte[] receiveByte = new Byte[1024];
              // clientSocketInThread.Receive(receiveByte);
              // String receivedStr = ASCIIEncoding.ASCII.GetString(receiveByte);

              clientSocketInThread.Shutdown(SocketShutdown.Both);
              clientSocketInThread.Close();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
          };
        };

        new Thread(
          socketProcessor(clientSocket)
        ).Start();
      }
    }
  }
}
