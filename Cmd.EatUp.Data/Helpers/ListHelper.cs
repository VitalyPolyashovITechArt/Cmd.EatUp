using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmd.EatUp.Helpers
{

        public static class ListHelper
        {
            public static List<T> Shuffle<T>(this List<T> list, int size)
            {
                Random rnd = new Random();
                var res = new T[size];

                res[0] = list[0];
                for (int i = 1; i < size; i++)
                {
                    int j = rnd.Next(i);
                    res[i] = res[j];
                    res[j] = list[i];
                }
                return res.ToList();
            }
        }
}