namespace FainEngine_v2.Utils
{
    public class Array2D<T>
    {
        public T[] Data { get; init; }

        public int X_Size { get; init; }
        public int Y_Size { get; init; }
        public int Size { get; init; }

        public Array2D(T[] data, int x_size, int y_size)
        {
            X_Size = x_size;
            Y_Size = y_size;
            Size = X_Size * Y_Size;

            Data = data;
        }

        public Array2D(int x_size, int y_size)
        {
            X_Size = x_size;
            Y_Size = y_size;
            Size = X_Size * Y_Size;

            Data = new T[x_size * y_size];
        }

        public T this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public T this[int x, int y]
        {
            get
            {
                if (x < 0 || x > X_Size ||
                    y < 0 || y > Y_Size)
                    throw new IndexOutOfRangeException($"Index out of Range XPos_px:{x}/{X_Size} YPox_px:{y}/{Y_Size}");

                return Data[x + y * X_Size];
            }
            set
            {
                if (x < 0 || x > X_Size ||
                    y < 0 || y > Y_Size)
                    throw new IndexOutOfRangeException($"Index out of Range XPos_px:{x}/{X_Size} YPox_px:{y}/{Y_Size}");

                Data[x + y * X_Size] = value;
            }
        }

        public T this[uint x, uint y]
        {
            get
            {
                if (x > X_Size ||
                    y > Y_Size)
                    throw new IndexOutOfRangeException($"Index out of Range XPos_px:{x}/{X_Size} YPox_px:{y}/{Y_Size}");

                return Data[x + y * X_Size];
            }
            set
            {
                if (x > X_Size ||
                    y > Y_Size)
                    throw new IndexOutOfRangeException($"Index out of Range XPos_px:{x}/{X_Size} YPox_px:{y}/{Y_Size}");

                Data[x + y * X_Size] = value;
            }
        }
    }
}
