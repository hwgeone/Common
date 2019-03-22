using System;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

//using (TransactionScope scope = new TransactionScope())
//           {
//               var queueManager = new MessageQueueBase<SampleModel>(@".\private$\test");

//               queueManager.MessageReceived += QueueManager_MessageReceived1;
//               for (int i = 0; i < 5; i++)
//               {
//                   queueManager.SendMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });
//               }

//               scope.Complete();
//           }

 //private static void QueueManager_MessageReceived1(object sender, MessageReceivedEventArgs<SampleModel> e)
 //       {
 //           Console.WriteLine("Message received {0} {1}", e.Message.ID, e.Message.TimeCreated.ToString("yyyy-MM-dd HH:mm:ss.fff"));
 //       }

namespace Infrastructure.MessageQueueHelper
{
    public delegate void MessageReceivedEventHandler<T>(Object sender, MessageReceivedEventArgs<T> e);
    public class MessageQueueBase<T> : MessageQueue where T : class, new()
    {
        public event MessageReceivedEventHandler<T> MessageReceived;
        public MessageQueueBase(String path)
            : base(path)
        {
            this.Formatter = new JsonMessageFormatter<T>(Encoding.UTF8);
            try
            {
                this.ReceiveCompleted += MessageQueueExtended_ReceiveCompleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            this.BeginReceive();
        }

        private void MessageQueueExtended_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                MessageQueue queue = (MessageQueue)sender;
                Message message = queue.EndReceive(e.AsyncResult);
                if (message != null && message.Body != null)
                {
                    var body = message.Body as T;
                    if (message != null)
                    {
                        OnMessageReceived(new MessageReceivedEventArgs<T>(body));
                    }
                }

                this.BeginReceive();
            }
            catch (MessageQueueException ex)
            {
                //Log exception
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
            }
        }

        public void SendMessage(T message)
        {
            this.Send(message, MessageQueueTransactionType.Automatic);
        }

        public T ReceiveMessageAsync()
        {
            return this.BeginReceive() as T;
        }

        public T ReceiveMessage()
        {
            return this.Receive(MessageQueueTransactionType.Automatic) as T;
        }
    }
}
