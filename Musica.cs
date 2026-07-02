using System;
using System.Runtime.InteropServices;
using System.Threading;

public static class Musica
{
    /// <summary>
    /// Toca uma pequena melodia utilizando apenas Console.Beep().
    /// </summary>
    public static void Tocar()
    {
        int[,] melodia =
        {
            { 262, 250 }, // C4
            { 294, 250 }, // D4
            { 330, 250 }, // E4
            { 349, 250 }, // F4
            { 392, 400 }, // G4
            {   0, 150 }, // Pausa

            { 392, 250 }, // G4
            { 440, 250 }, // A4
            { 494, 250 }, // B4
            { 523, 500 }, // C5
            {   0, 200 }, // Pausa

            { 523, 200 }, // C5
            { 494, 200 }, // B4
            { 440, 200 }, // A4
            { 392, 400 }, // G4
            { 349, 250 }, // F4
            { 330, 250 }, // E4
            { 294, 250 }, // D4
            { 262, 600 }  // C4
        };

        for (int i = 0; i < melodia.GetLength(0); i++)
        {
            int frequencia = melodia[i, 0];
            int duracao = melodia[i, 1];

            if (frequencia == 0)
            {
                Thread.Sleep(duracao);
            }
            else if (OperatingSystem.IsWindows())
            {
                Console.Beep(frequencia, duracao);
            }
            else
            {
                Thread.Sleep(duracao);
            }

            Thread.Sleep(30);
        }
    }
}