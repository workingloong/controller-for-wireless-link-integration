using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Cshapshiyan_1
{
    class Tool
    {
        public static byte[] StructToBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
           // int size = 1324;
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size); //分配结构体大小的内存空间
            Marshal.StructureToPtr(obj, structPtr, false); //将结构体拷到分配好的内存空间
            Marshal.Copy(structPtr, bytes, 0, size);       //从内存空间拷到byte数组
            Marshal.FreeHGlobal(structPtr);                //释放内存空间
            return bytes;
            //int size = Marshal.SizeOf(obj);
            //size = 1324;
            //byte[] bytes = new byte[size];
            //IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            //Marshal.StructureToPtr(obj, arrPtr, true);
            //return bytes;

        }

        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
                return null;
            IntPtr structPtr = Marshal.AllocHGlobal(size); //分配结构大小的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);       //将byte数组拷到分配好的内存空间
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
            //IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            //return (object)Marshal.PtrToStructure(arrPtr, typeof(object));

        }

    }
}
