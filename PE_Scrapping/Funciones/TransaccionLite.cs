﻿using PE_Scrapping.Entidades;
using System;
using System.Data.SQLite;
using System.IO;

namespace PE_Scrapping.Funciones
{
    public class TransaccionLite
    {
        private static bool IsDbRecentlyCreated = false;
        private static string _dbName;
        public TransaccionLite(string DBName)
        {
            _dbName = DBName;
            if (!File.Exists(Path.GetFullPath(DBName)))
            {
                SQLiteConnection.CreateFile(DBName);
                IsDbRecentlyCreated = true;
            }
            using (var ctx = GetInstance(DBName))
            {
                if (IsDbRecentlyCreated)
                {
                    using (var command = new SQLiteCommand(ReadQuery("create_db_objects"), ctx))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public static SQLiteConnection GetInstance(string DBName)
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );
            db.Open();
            return db;
        }
        public void GuardarLocales(Local locales, string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("insert_local");
                    using (var transaction = db.BeginTransaction())
                    {
                        locales.locales.ForEach(l =>
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SQLiteParameter("@local_codigo", l.CCODI_LOCAL));
                            cmd.Parameters.Add(new SQLiteParameter("@local_ubigeo", l.CCODI_UBIGEO));
                            cmd.Parameters.Add(new SQLiteParameter("@local_nombre", l.TNOMB_LOCAL));
                            cmd.Parameters.Add(new SQLiteParameter("@local_direccion", l.TDIRE_LOCAL));
                            cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                            cmd.ExecuteNonQuery();
                        });
                        transaction.Commit();
                    }
                }
            }
        }
        public void GuardarMesas(Mesa mesas, string codigo_local, string codigo_ubigeo, string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("insert_mesa");
                    using (var transaction = db.BeginTransaction())
                    {
                        mesas.mesasVotacion.ForEach(l =>
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SQLiteParameter("@local_ubigeo", codigo_ubigeo));
                            cmd.Parameters.Add(new SQLiteParameter("@local_codigo", codigo_local));
                            cmd.Parameters.Add(new SQLiteParameter("@mesa_numero", l.NUMMESA));
                            cmd.Parameters.Add(new SQLiteParameter("@mesa_procesado", l.PROCESADO));
                            cmd.Parameters.Add(new SQLiteParameter("@mesa_imagen", l.IMAGEN));
                            cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                            cmd.ExecuteNonQuery();
                        });
                        transaction.Commit();
                    }
                }
            }
        }
        private SQLiteCommand SetCommandActa(SQLiteConnection connection,
            string mesa_numero, string acta_numero, string acta_imagen, 
            int habiles_numero, int votantes_numero, string eleccion, string tipo_proceso)
        {
            SQLiteCommand command = new(connection);
            command.CommandText = ReadQuery("insert_acta");
            command.Parameters.Add(new SQLiteParameter("@mesa_numero", mesa_numero));
            command.Parameters.Add(new SQLiteParameter("@acta_numero", acta_numero));
            command.Parameters.Add(new SQLiteParameter("@acta_imagen", acta_imagen));
            command.Parameters.Add(new SQLiteParameter("@habiles_numero", habiles_numero));
            command.Parameters.Add(new SQLiteParameter("@votantes_numero", votantes_numero));
            command.Parameters.Add(new SQLiteParameter("@eleccion", eleccion));
            command.Parameters.Add(new SQLiteParameter("@tipo_proceso", "PRE"));
            return command;
        }
        private SQLiteCommand SetCommandVoto(SQLiteConnection connection,
            string mesa_numero, string acta_numero, string auto_nombre,
            int lista_numero, int votos_total, string eleccion, string tipo_proceso)
        {
            SQLiteCommand command = new(connection);
            command.CommandText = ReadQuery("insert_voto");
            command.Parameters.Add(new SQLiteParameter("@mesa_numero", mesa_numero));
            command.Parameters.Add(new SQLiteParameter("@acta_numero", acta_numero));
            command.Parameters.Add(new SQLiteParameter("@auto_nombre", auto_nombre));
            command.Parameters.Add(new SQLiteParameter("@lista_numero", lista_numero));
            command.Parameters.Add(new SQLiteParameter("@votos_total", votos_total));
            command.Parameters.Add(new SQLiteParameter("@eleccion", eleccion));
            command.Parameters.Add(new SQLiteParameter("@tipo_proceso", tipo_proceso));
            return command;
        }
        private SQLiteCommand SetCommandUbigeo(SQLiteConnection connection,
            string ubigeo_codigo, string ubigeo_descripcion, string ubigeo_padre, string eleccion, int nivel, string ambito
           )
        {
            SQLiteCommand command = new(connection);
            command.CommandText = ReadQuery("insert_ubigeo");
            command.Parameters.Add(new SQLiteParameter("@ubigeo_codigo", ubigeo_codigo));
            command.Parameters.Add(new SQLiteParameter("@ubigeo_descripcion", ubigeo_descripcion));
            command.Parameters.Add(new SQLiteParameter("@ubigeo_padre", ubigeo_padre));
            command.Parameters.Add(new SQLiteParameter("@eleccion", eleccion));
            command.Parameters.Add(new SQLiteParameter("@nivel", nivel));
            command.Parameters.Add(new SQLiteParameter("@ambito", ambito));
            return command;
        }
        public void GuardarMesaDetalle(MesaDetalle mesaDetalle, string numero_mesa, string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var transaction = db.BeginTransaction())
                {
                    if (mesaDetalle.procesos.generalPre != null)
                    {
                        SetCommandActa(db,
                        numero_mesa,
                        mesaDetalle.procesos.generalPre.presidencial.CCOPIA_ACTA,
                        mesaDetalle.procesos.generalPre.imageActa,
                        mesaDetalle.procesos.generalPre.presidencial.NNUME_HABILM,
                        mesaDetalle.procesos.generalPre.presidencial.TOT_CIUDADANOS_VOTARON,
                        opcion, "PRE").ExecuteNonQuery();
                        mesaDetalle.procesos.generalPre.votos.ForEach(v =>
                        {
                            SetCommandVoto(db,
                                numero_mesa,
                                mesaDetalle.procesos.generalPre.presidencial.CCOPIA_ACTA,
                                v.AUTORIDAD,
                                v.NLISTA,
                                Convert.ToInt32(v.congresal),
                                opcion, "PRE"
                                ).ExecuteNonQuery();
                        });
                    }
                    if (mesaDetalle.procesos.generalCon != null)
                    {
                        SetCommandActa(db,
                        numero_mesa,
                        mesaDetalle.procesos.generalCon.congresal.CCOPIA_ACTA,
                        mesaDetalle.procesos.generalCon.imageActa,
                        mesaDetalle.procesos.generalCon.congresal.NNUME_HABILM,
                        mesaDetalle.procesos.generalCon.congresal.TOT_CIUDADANOS_VOTARON,
                        opcion, "CON").ExecuteNonQuery();
                        mesaDetalle.procesos.generalCon.votos.ForEach(v =>
                        {
                            SetCommandVoto(db,
                                numero_mesa,
                                mesaDetalle.procesos.generalCon.congresal.CCOPIA_ACTA,
                                v.AUTORIDAD,
                                v.NLISTA,
                                Convert.ToInt32(v.congresal),
                                opcion, "CON"
                                ).ExecuteNonQuery();
                        });
                    }
                    if (mesaDetalle.procesos.generalPar != null)
                    {
                        SetCommandActa(db,
                        numero_mesa,
                        mesaDetalle.procesos.generalPar.parlamento.CCOPIA_ACTA,
                        mesaDetalle.procesos.generalPar.imageActa,
                        mesaDetalle.procesos.generalPar.parlamento.NNUME_HABILM,
                        mesaDetalle.procesos.generalPar.parlamento.TOT_CIUDADANOS_VOTARON,
                        opcion, "PAR").ExecuteNonQuery();
                        mesaDetalle.procesos.generalPar.votos.ForEach(v =>
                        {
                            SetCommandVoto(db,
                                numero_mesa,
                                mesaDetalle.procesos.generalPar.parlamento.CCOPIA_ACTA,
                                v.AUTORIDAD,
                                v.NLISTA,
                                Convert.ToInt32(v.congresal),
                                opcion, "PAR"
                                ).ExecuteNonQuery();
                        });
                    }
                    transaction.Commit();
                }
            }
        }
        public void GuardarUbigeos(Ubigeo ubigeos, string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var transaction = db.BeginTransaction())
                {
                    ubigeos.ubigeos.nacional.departments.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                            o.CDGO_DEP,
                            o.DESC_DEP,
                            o.CDGO_PADRE,
                            opcion, 1, "P"
                            ).ExecuteNonQuery();
                    });
                    ubigeos.ubigeos.nacional.provinces.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                            o.CDGO_PROV,
                            o.DESC_PROV,
                            o.CDGO_PADRE,
                            opcion, 2, "P"
                            ).ExecuteNonQuery();
                    });
                    ubigeos.ubigeos.nacional.districts.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                               o.CDGO_DIST,
                               o.DESC_DIST,
                               o.CDGO_PADRE,
                               opcion, 3, "P"
                               ).ExecuteNonQuery();
                    });
                    ubigeos.ubigeos.extranjero.continents.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                               o.CDGO_DEP,
                               o.DESC_DEP,
                               o.CDGO_PADRE,
                               opcion, 1, "E"
                               ).ExecuteNonQuery();
                    });
                    ubigeos.ubigeos.extranjero.countries.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                               o.CDGO_PROV,
                               o.DESC_PROV,
                               o.CDGO_PADRE,
                               opcion, 2, "E"
                               ).ExecuteNonQuery();
                    });
                    ubigeos.ubigeos.extranjero.states.ForEach(o =>
                    {
                        SetCommandUbigeo(db,
                                  o.CDGO_DIST,
                                  o.DESC_DIST,
                                  o.CDGO_PADRE,
                                  opcion, 3, "E"
                                  ).ExecuteNonQuery();
                    });
                    transaction.Commit();
                }
            }
        }
        public void LimpiarData(string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public void LimpiarDataUbigeo(string opcion)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data_ubigeo");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public void LimpiarDataMesa(string opcion, string mesa_numero)
        {
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("purge_data_mesa");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.Parameters.Add(new SQLiteParameter("@mesa_numero", mesa_numero));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public void LimpiarDataDetalleUbigeo(string opcion, string mesa_numero)
        {
            string level = string.Empty;
            using (var db = GetInstance(_dbName))
            {
                using (var cmd = new SQLiteCommand(db))
                {
                    cmd.CommandText = ReadQuery("get_level");
                    cmd.Parameters.Add(new SQLiteParameter("@codigo_ubigeo", mesa_numero));
                    object result = cmd.ExecuteScalar();
                    level = (result == null ? string.Empty : result.ToString());

                    cmd.Parameters.Clear();
                    cmd.CommandText = ReadQuery("purge_data_detalle_ubigeo");
                    using (var transaction = db.BeginTransaction())
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@level", level));
                        cmd.Parameters.Add(new SQLiteParameter("@eleccion", opcion));
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }

        private string ReadQuery(string query_name)
        {
            var query_path = string.Concat(@"Script\SQLite\", query_name, ".sql");
            var query = "";
            var line = "";
            using (var reader = new StreamReader(Path.GetFullPath(query_path)))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    query += line;
                }
            }
            return query;
        }
    }
}