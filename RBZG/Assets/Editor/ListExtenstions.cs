using System.Collections.Generic;

public static class ListExtensions
{
    public static void ReverseList<T>(this List<T> list)
    {
        int i = 0;
        int j = list.Count - 1;
        while (i < j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
            i++;
            j--;
        }
    }
}
