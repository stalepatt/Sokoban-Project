using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
   struct Box
    {
        public int X;
        public int Y;
        // 박스가 골 위에 있는지를 저장하기 위한 변수
        public bool IsOnGoal;
    }
}
