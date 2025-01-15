using UnityEngine;

public static class FieldSerializer
{
    [System.Serializable]
    private class SerializedFieldData
    {
        public int[] FlattenedField;
        public int Rows;
        public int Cols;

        public SerializedFieldData(int[] flattenedField, int rows, int cols)
        {
            FlattenedField = flattenedField;
            Rows = rows;
            Cols = cols;
        }
    }
    /// <summary>
    /// Converts [,] arr to json string
    /// </summary>
    /// <returns>Json string</returns>
    public static string Serialize(int[,] field)
    {
        int rows = field.GetLength(0);
        int cols = field.GetLength(1);
        int[] flattenedField = new int[rows * cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                flattenedField[i * cols + j] = field[i, j];
            }
        }

        return JsonUtility.ToJson(new SerializedFieldData(flattenedField, rows, cols));
    }
    /// <summary>
    /// Converts json string to [,] arr
    /// </summary>
    /// <param name="json">Serialized matrix</param>
    /// <returns>Field matrix</returns>
    public static int[,] Deserialize(string json)
    {
        SerializedFieldData data = JsonUtility.FromJson<SerializedFieldData>(json);
        int[,] field = new int[data.Rows, data.Cols];

        for (int i = 0; i < data.Rows; i++)
        {
            for (int j = 0; j < data.Cols; j++)
            {
                field[i, j] = data.FlattenedField[i * data.Cols + j];
            }
        }

        return field;
    }
}
