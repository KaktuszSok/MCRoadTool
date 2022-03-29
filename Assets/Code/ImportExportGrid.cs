using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Code
{
    public static class ImportExportGrid
    {
        private static readonly Encoding encoding = Encoding.Unicode;
    
        public static string ExportGrid(CellGrid grid)
        {
            using MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms, encoding))
            {
                WriteMapData(grid, bw);
                WriteGridData(grid, bw);
                WriteTerrainData(grid, bw);

                bw.Write('0');
            }
            ms.Flush();
            byte[] bytes = ms.ToArray();

            using MemoryStream gzOut = new MemoryStream();
            using (GZipStream gz = new GZipStream(gzOut, CompressionLevel.Fastest))
            {
                gz.Write(bytes, 0, bytes.Length);
            }
            gzOut.Flush();

            byte[] bytesCompressed = gzOut.ToArray();
            return Convert.ToBase64String(bytesCompressed);
        }

        private static void WriteMapData(CellGrid grid, BinaryWriter bw)
        {
            bw.Write('M');
        
            bw.Write(grid.chunks.GetLength(0));
            bw.Write(grid.chunks.GetLength(1));
        }

        private static void WriteGridData(CellGrid grid, BinaryWriter bw)
        {
            bw.Write('G');

            foreach (CellChunk gridChunk in grid.chunks)
            {
                foreach (GridCell cell in gridChunk.cells)
                {
                    bw.Write((byte)cell.state);
                }
            }
        }
        
        private static void WriteTerrainData(CellGrid grid, BinaryWriter bw)
        {
            bw.Write('T');

            foreach (CellChunk gridChunk in grid.chunks)
            {
                foreach (GridCell cell in gridChunk.cells)
                {
                    bw.Write(cell.height);
                    bw.Write(cell.isGround);
                }
            }
        }

        public static bool ImportGrid(CellGrid grid, string data_base64)
        {
            //PrintStream(data_base64);
            try
            {
                byte[] bytesCompressed = Convert.FromBase64String(data_base64);
                using GZipStream gz = new GZipStream(new MemoryStream(bytesCompressed), CompressionMode.Decompress);
                using BinaryReader br = new BinaryReader(gz, encoding);

                char c;
                while ((c = br.ReadChar()) != '0')
                {
                    switch (c)
                    {
                        case 'M':
                            ReadMapData(grid, br);
                            break;
                        case 'G':
                            ReadGridData(grid, br);
                            break;
                        case 'T':
                            ReadTerrainData(grid, br);
                            break;
                        default:
                            Debug.LogError($"Unknown data block: {c}. Terminating import.");
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        private static void ReadMapData(CellGrid grid, BinaryReader br)
        {
            int sizeX = br.ReadInt32();
            int sizeY = br.ReadInt32();
            grid.aspectRatio = (float)sizeX / sizeY;
            grid.size = sizeY;
            grid.RegenerateGrid();
            grid.RealignCam();
        }
    
        private static void ReadGridData(CellGrid grid, BinaryReader br)
        {
            foreach (CellChunk gridChunk in grid.chunks)
            {
                foreach (GridCell cell in gridChunk.cells)
                {
                    cell.SetState((CellState)br.ReadByte(), true);
                }
            }
        }
        
        private static void ReadTerrainData(CellGrid grid, BinaryReader br)
        {
            foreach (CellChunk gridChunk in grid.chunks)
            {
                foreach (GridCell cell in gridChunk.cells)
                {
                    cell.SetHeight(br.ReadInt16(), false);
                    cell.SetGround(br.ReadBoolean(), true);
                }
            }
        }
    }
}
