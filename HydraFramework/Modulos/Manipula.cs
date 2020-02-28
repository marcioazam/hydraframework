using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HydraFramework.Modulos
{
    internal class Manipula
    {
        public enum TipoConsulta
        {
            Select = 1,
            Definicao = 2,
            Insert = 3,
            Update = 4,
            Delete = 5
        }

        public static void Consulta(out string comandoSQL, Type tipo, TipoConsulta tipoConsulta, int? top = null, string colunas = "*", string parametros = "", string condicoes = "")
        {
            string nomeTabela = Valida.NomeTabela(tipo);

            switch (tipoConsulta)
            {
                case TipoConsulta.Select:
                    string topLinhas;
                    TopLinhas(top, out topLinhas);
                    comandoSQL = $"SELECT{topLinhas}{colunas} FROM {nomeTabela} {condicoes};";
                    break;
                case TipoConsulta.Definicao:
                    comandoSQL = $"SELECT ID = (COLUMNPROPERTY(OBJECT_ID(B.TABLE_SCHEMA + '.' + B.TABLE_NAME), COLUMN_NAME, 'IsIdentity')) FROM " +
                        $"INFORMATION_SCHEMA.KEY_COLUMN_USAGE A INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS B ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME " +
                        $"WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND B.TABLE_NAME = '{nomeTabela}';";
                    break;
                case TipoConsulta.Insert:
                    comandoSQL = $"INSERT INTO {nomeTabela} ({colunas}) VALUES ({parametros}){condicoes}";
                    break;
                case TipoConsulta.Update:
                    comandoSQL = $"UPDATE {nomeTabela} SET {parametros} WHERE {condicoes}";
                    break;
                case TipoConsulta.Delete:
                    comandoSQL = $"DELETE {nomeTabela} WHERE {condicoes}";
                    break;
                default:
                    comandoSQL = "";
                    break;
            }
        }

        public static void TopLinhas(int? top, out string topLinhas)
        {
            topLinhas = " ";

            if (top != null)
            {
                topLinhas = $" TOP {top} ";
            }
        }

        public static string Delete(Type tipo, string nomePK, string valorPK)
        {
            string retornoDelete;

            Consulta(out retornoDelete, tipo, TipoConsulta.Delete, condicoes: $"{nomePK} = {valorPK};");

            return retornoDelete;
        }

        public static string Update(Type tipo, List<string> NomeColunas, string nomePK, string valorPK, bool contemID = true)
        {
            string stringConsulta;
            string retornoSave;

            Consulta(out retornoSave, tipo, TipoConsulta.Select, condicoes: $"WHERE {nomePK} = SCOPE_IDENTITY()");

            var colunas = NomeColunas.ToArray();
            for(int i = 0; i < colunas.Length; i++)
            {
                colunas[i] += $"=@{colunas[i]}";
            }

            string parametros = string.Join(",", colunas);

            if (contemID == false)
            {
                parametros += $",{nomePK}=@{nomePK}";
            }
          
            Consulta(out stringConsulta, tipo, TipoConsulta.Update, parametros: parametros, condicoes: $"{nomePK} = {valorPK}; {retornoSave}");

            return stringConsulta;
        }

        public static string Insert(Type tipo, List<string> NomeColunas, string nomePK = "", bool contemID = true)
        {
            string stringConsulta;
            string retornoSave;
            string colunas;
            string parametros = "";

            Consulta(out retornoSave, tipo, TipoConsulta.Select, condicoes: $"WHERE {nomePK} = SCOPE_IDENTITY()");

            colunas = string.Join(",", NomeColunas.ToArray());

            foreach (var item in NomeColunas)
            {
                parametros += $"@{item},";
            }
            parametros = parametros.Remove(parametros.Length - 1, 1);

            if (contemID == false)
            {
                colunas += ", " + nomePK;
                parametros += ", @" + nomePK;
            }

            Consulta(out stringConsulta, tipo, TipoConsulta.Insert, colunas: colunas, parametros: parametros, condicoes: $"; {retornoSave}");

            return stringConsulta;
        }
    }
}
