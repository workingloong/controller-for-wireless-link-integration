using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace Cshapshiyan_1
{
    public partial class Painter
    {
        public String name;
        public int localX;
        public int localY;
        public int yFrom;
        public int yTo;
        public int length = 110;
        public int weight = 110;
  //      public double step;
        public Queue dataQ = new Queue(); // FIFO队列，用于存储画图的值
        public Painter(String s)
        {
            name = s;
            yFrom = 0;   
        }
        public Painter() {
            name = "painter";
            yFrom = 0;
        }
        public void init_data()
        {
            for (int i = 0; i < 15; i++)
            {
                if(dataQ.Count == 15)
                {
                    dataQ.Dequeue(); // 如果队列长度超过15，则删除队首的值，在队尾加入新的值
                }
                dataQ.Enqueue(0.01);
            }

        }
    }
}
