﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;


namespace Compacter
{
    class Program
    {
        private static InternalConfig config = new InternalConfig();

        public class InternalConfig
        {
            public InternalConfig()
            {
                try
                {
                    Conn = ConfigurationManager.AppSettings["conn"];
                    TableSelect = ConfigurationManager.AppSettings["tableSelect"];
                    TableUpdate = ConfigurationManager.AppSettings["tableUpdate"];
                    SelectProps = ConfigurationManager.AppSettings["updateProps"].Split(',').ToList();
                    UpdadeProps = ConfigurationManager.AppSettings["selectProps"].Split(',').ToList();
                    ConsultQuery = ConfigurationManager.AppSettings["consultQuery"];

                    SelectQuery = "Select ";
                    SelectProps.ForEach(x =>
                    {
                        SelectQuery += x + ",";
                    });

                    SelectQuery = SelectQuery.Substring(0, SelectQuery.Length - 1);

                    SelectQuery += " from " + TableSelect + " where " + SelectProps[0] + "=@" + SelectProps[0];

                    //--
                    UpdateQuery = "Update " + TableUpdate + " set " + UpdadeProps[1] + "=@FileData Where " + UpdadeProps[0] + "=@Id";

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public string Conn { get; protected set; }
            public string TableSelect { get; protected set; }
            public string TableUpdate { get; protected set; }
            public List<string> SelectProps { get; protected set; }
            public List<string> UpdadeProps { get; protected set; }
            public string SelectQuery { get; protected set; }
            public string UpdateQuery { get; protected set; }
            public string InsertQuery { get; set; } = "Insert Into TestFile (Id_File,Nome,FileData) values (NEWID(),@Nome,@FileData)";
            public string DeleteQuery { get; protected set; } = "Delete from TestFile";
            public string ConsultQuery { get; protected set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("PDF Compacter ver 0.1\n");

            InternalConfig config = new InternalConfig();

            string cmd = string.Empty;

            Console.Write("\n->");

            while (!string.IsNullOrEmpty(cmd = Console.ReadLine()))
            {
                switch (cmd)
                {
                    case "exit":
                        break;

                    case "start":
                        Compactar();
                        break;

                    case "upload":
                        Uploader();
                        break;

                    case "delete":
                        Delete();
                        break;

                    case "optimize":
                        Optimize();
                        break;

                    case "data":
                        Fecha();
                        break;

                    case "codigo":
                        UpdateCodigo();
                        break;
                    default:
                        Console.WriteLine("Comando desconhecido.");
                        break;
                }

                Console.Write("\n->");
            }
        }

        public static void Fecha()
        {
            var inicio = DateTime.Now;
            Console.WriteLine("Data Inicio: " + DetalharData(inicio));
        }

        private static string DetalharData(DateTime data)
        {
            return
                $"{data.DayOfWeek.ToString()},{data.Day}/{data.Month}/{data.Year} - {data.Hour}:{data.Minute}:{data.Second}:{data.Millisecond}";
        }

        private static string DetalharData(TimeSpan data)
        {
            return
                $"{data.Hours}:{data.Minutes}:{data.Seconds}:{data.Milliseconds}";
        }

        private static void UpdateCodigo()
        {
            Console.WriteLine("Escolha a query");
            var text = Console.ReadLine();
        }

        public static void LimparLinea()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        private static void Compactar()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pdf");

            foreach (string s in files)
            {
                //UtilzariText(s);
                UtilizarGhostScript(s);
            }

            Console.WriteLine("Terminado");
        }

        private static void UtilizarGhostScript(string path)
        {
            //GhostscriptVersionInfo gv = GhostscriptVersionInfo.GetLastInstalledVersion();

            GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(Directory.GetFiles(Directory.GetCurrentDirectory(), "gsdll32.dll").FirstOrDefault());

            using (GhostscriptProcessor processor = new GhostscriptProcessor(gvi, true))
            {
                //processor.Processing += new GhostscriptProcessorProcessingEventHandler(processor_Processing);

                var fi = new System.IO.FileInfo(path);

                List<string> switches = new List<string>();
                switches.Add("-empty");
                //switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                //switches.Add(@"-sFONTPATH=" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts));
                //switches.Add("-dFirstPage=" + pageFrom.ToString());
                //switches.Add("-dLastPage=" + pageTo.ToString());
                switches.Add("-sDEVICE=pdfwrite");
                switches.Add("-CompressPages=true");
                switches.Add("-dCompatibilityLevel=1.5");
                switches.Add("-dPDFSETTINGS=/screen");
                switches.Add("-dEmbedAllFonts=false");
                switches.Add("-dSubsetFonts=false");
                //switches.Add("-dColorImageDownsampleType=/Bicubic");
                //switches.Add("-dColorImageResolution=144");
                //switches.Add("-dGrayImageDownsampleType=/Bicubic");
                //switches.Add("-dGrayImageResolution=144");
                //switches.Add("-dMonoImageDownsampleType=/Bicubic");
                //switches.Add("-dMonoImageResolution=144");
                switches.Add(@"-sOutputFile=D:\Projecto Compacter\Ghost" + fi.Name);
                switches.Add(@"-f");
                switches.Add(path);

                // if you dont want to handle stdio, you can pass 'null' value as the last parameter
                //LogStdio stdio = new LogStdio();
                processor.StartProcessing(switches.ToArray(), null);
            }
        }

        private static void OptimizarDump()
        {
            var dump = Directory.GetCurrentDirectory() + @"\dump";
            var dumped = Directory.GetCurrentDirectory() + @"\dumped";
            GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(Directory.GetFiles(Directory.GetCurrentDirectory(), "gsdll32.dll").FirstOrDefault());

            using (GhostscriptProcessor processor = new GhostscriptProcessor(gvi, true))
            {
                //processor.Processing += new GhostscriptProcessorProcessingEventHandler(processor_Processing);

                List<string> switches = new List<string>();
                switches.Add("-empty");
                //switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                //switches.Add(@"-sFONTPATH=" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts));
                //switches.Add("-dFirstPage=" + pageFrom.ToString());
                //switches.Add("-dLastPage=" + pageTo.ToString());
                switches.Add("-sDEVICE=pdfwrite");
                switches.Add("-CompressPages=true");
                switches.Add("-dCompatibilityLevel=1.5");
                switches.Add("-dPDFSETTINGS=/ebook");
                switches.Add("-dEmbedAllFonts=false");
                switches.Add("-dSubsetFonts=false");

                switches.Add(@"-sOutputFile=" + dumped);
                switches.Add(@"-f");
                switches.Add(dump);

                // if you dont want to handle stdio, you can pass 'null' value as the last parameter
                //LogStdio stdio = new LogStdio();
                processor.StartProcessing(switches.ToArray(), null);
            }
        }

        private static void Uploader()
        {
            InternalConfig config = new InternalConfig();

            Console.WriteLine("Subindo Ficheiros:");

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pdf");

            foreach (string s in files)
            {
                var fi = new System.IO.FileInfo(s);

                Console.Write("-" + fi.Name);

                try
                {
                    using (SqlConnection conn = new SqlConnection(config.Conn))
                    {
                        conn.Open();

                        string query = config.InsertQuery;

                        SqlCommand cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@Nome", fi.Name);
                        SqlParameter fileData = cmd.Parameters.Add("@FileData", SqlDbType.VarBinary);
                        fileData.Value = File.ReadAllBytes(s);

                        cmd.ExecuteNonQuery();
                        Console.Write(" - Subido.\n");
                    }
                }
                catch (SqlException ex)
                {
                    Console.Write(" - " + ex.Message + ".\n");
                }

            }

            Console.WriteLine("\n " + files.Length + " Ficheiros Subidos");
        }

        private static void Delete()
        {
            try
            {
                InternalConfig config = new InternalConfig();

                using (SqlConnection conn = new SqlConnection(config.Conn))
                {
                    conn.Open();

                    string query = config.DeleteQuery;

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.ExecuteNonQuery();

                    Console.WriteLine("\n Ficheiros Eliminados");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Optimize()
        {
            var inicio = DateTime.Now;

            var dump = Directory.GetCurrentDirectory() + @"\dump";
            var dumped = Directory.GetCurrentDirectory() + @"\dumped";

            Console.WriteLine("Optimizando Ficheiros:");

            var ids = new List<Guid>();

            try
            {
                using (SqlConnection conn = new SqlConnection(config.Conn))
                {
                    conn.Open();

                    string query = config.ConsultQuery;

                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    ids.AddRange(from DataRow dr in dt.Rows select (Guid)dr[0]);

                }
            }
            catch (SqlException ex)
            {
                Console.Write(" - " + ex.Message + ".\n");
            }


            Console.WriteLine(ids.Count + " ficheiros a optimizar.");

            var i = 1;


            ids.ForEach(x =>
            {
                //Console.WriteLine(" - Optimizando ficheiros.");
                try
                {
                    using (var conn = new SqlConnection(config.Conn))
                    {
                        DateTime begin = DateTime.Now;
                        conn.Open();

                        var query = config.SelectQuery;

                        var cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@" + config.SelectProps[0], x.ToString());

                        SqlDataReader reader = cmd.ExecuteReader();
                        //--Descargar--//
                        if (reader.Read())
                        {
                            File.WriteAllBytes(dump, (byte[])reader[config.SelectProps[1]]);
                        }

                        reader.Dispose();
                        //--Optimizar--//



                        OptimizarDump();
                        //--Update--//



                        var uCommand = new SqlCommand(config.UpdateQuery, conn);
                        uCommand.Parameters.AddWithValue("@Id", x.ToString());
                        var fileData = uCommand.Parameters.Add("@FileData", System.Data.SqlDbType.VarBinary);

                        fileData.Value = File.ReadAllBytes(dumped);

                        uCommand.ExecuteNonQuery();
                        DateTime end = DateTime.Now;
                        //LimparLinea();
                        Console.WriteLine("Ficheiros Optimizados ({0}/{1}) {2} seg", i, ids.Count, (end - begin).Seconds);
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(" - " + ex.Message + ".\n");
                }
            });


            var fim = DateTime.Now;
            var duracao = fim - inicio;
            Console.WriteLine("Data Inicio: " + DetalharData(inicio));
            Console.WriteLine("Data Inicio: " + DetalharData(fim));
            Console.WriteLine("Duração: " + DetalharData(duracao));
            Console.WriteLine("\nOptimização Terminada");
        }
    }
}
