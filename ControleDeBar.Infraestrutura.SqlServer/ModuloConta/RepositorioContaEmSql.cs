
using System.Data;
using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloMesa;
using Microsoft.Data.SqlClient;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloConta
{
    public class RepositorioContaEmSql : IRepositorioConta
    {
        private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=controleDeBarDb;Integrated Security=True";
        public List<Conta> SelecionarContas()
        {
            var sqlSelecionarContas =
                @"SELECT 
                    CONT.[ID],
                    CONT.[TITULAR],
                    CONT.[MESA_ID],
                    CONT.[GARCOM_ID],
                    CONT.[DATAABERTURA],
                    CONT.[DATAFECHAMENTO],
                    CONT.[ESTAABERTA],
                    MESA.[NUMERO],
                    MESA.[CAPACIDADE],
                    GARCOM.[NOME],
                    GARCOM.[CPF]

                FROM 
                    [TBCONTA] AS CONT 

                LEFT JOIN
                    [TBMESA] AS MESA
                ON
                    MESA.ID = CONT.MESA_ID

                LEFT JOIN 
                    [TBGARCOM] AS GARCOM
                ON
                    GARCOM.ID = CONT.GARCOM_ID";

            SqlConnection conexaoComBanco  = new SqlConnection(connectionString);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarContas, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorContas = comandoSelecao.ExecuteReader();

            var contas = new List<Conta>();

            while (leitorContas.Read())
            {
                var conta = ConverterParaConta(leitorContas);

                contas.Add(conta);
            }

            conexaoComBanco.Close();

            return contas;
        }
        private Conta ConverterParaConta(SqlDataReader leitorContas)
        {
            Mesa? mesa= null;
            mesa = ConverterParaMesa(leitorContas);

            DateTime? fechamento = null;
            Garcom? garcom  = null;
            garcom = ConverterParaGarcom(leitorContas);

            
            var conta = new Conta(
                leitorContas["TITULAR"].ToString()!,
                mesa,
                garcom
                );

            conta.Id = Guid.Parse(leitorContas["ID"].ToString()!);

            return conta;
        }
        private Mesa ConverterParaMesa(SqlDataReader leitor)
        {
            var mesa = new Mesa
                (
                    Convert.ToInt32(leitor["NUMERO"])!,
                    Convert.ToInt32(leitor["CAPACIDADE"])!
                );

            mesa.Id = Guid.Parse(leitor["ID"].ToString()!);


            return mesa;
        }
        private Garcom ConverterParaGarcom(SqlDataReader leitor)
        {
            var garcom = new Garcom
                (
                    Convert.ToString(leitor["NOME"])!,
                    Convert.ToString(leitor["CPF"])!
                );

            garcom.Id = Guid.Parse(leitor["ID"].ToString()!);


            return garcom;
        }
        public void CadastrarConta(Conta conta)
        {
            var sqlInserirConta =
                @"INSERT INTO [TBCONTA]
                    (
                        [ID],
                        [TITULAR],
                        [MESA_ID],
                        [GARCOM_ID],
                        [DATAABERTURA],
                        [DATAFECHAMENTO],
                        [ESTAABERTA]
                    )
                VALUES
                    (
                        @ID,
                        @TITULAR,
                        @MESA_ID,
                        @GARCOM_ID,
                        @DATAABERTURA,
                        @DATAFECHAMENTO,
                        @ESTAABERTA
                    )";
        
            SqlConnection conexaComBanco = new SqlConnection(connectionString);
            SqlCommand comandoInsercao = new SqlCommand(sqlInserirConta, conexaComBanco);

            ConfigurarParametrosConta(conta, comandoInsercao);

            conexaComBanco.Open();
            comandoInsercao.ExecuteNonQuery();
            conexaComBanco.Close();
        }
        public Conta SelecionarPorId(Guid idRegistro)
        {
           var selecionarRegistroPorId =
                @"SELECT 
                    CONT.[ID],
                    CONT.[TITULAR],
                    CONT.[MESA_ID],
                    CONT.[GARCOM_ID],
                    CONT.[DATAABERTURA],
                    CONT.[DATAFECHAMENTO],
                    CONT.[ESTAABERTA],
                    MESA.[NUMERO],
                    MESA.[CAPACIDADE],
                    GARCOM.[NOME],
                    GARCOM.[CPF]
                FROM
                    [TBCONTA] AS CONT
                LEFT JOIN
                    [TBMESA] AS MESA
                ON
                    MESA.ID = CONT.MESA_ID
                LEFT JOIN
                    [TBGARCOM] AS GARCOM
                ON
                    GARCOM.ID = CONT.GARCOM_ID
                WHERE
                    CONT.ID = @ID";
            SqlConnection conexaoComBanco = new SqlConnection(connectionString);
            SqlCommand comandoSelecao = new SqlCommand(selecionarRegistroPorId, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

            conexaoComBanco.Open();
            SqlDataReader leitor = comandoSelecao.ExecuteReader();
            Conta? conta = null;
            if (leitor.Read())
            {
                conta = ConverterParaConta(leitor);
            }
            conexaoComBanco.Close();
            return conta!;

        }
        public List<Conta> SelecionarContasAbertas()
        {
            string sqlSelecionarTodos =
           @"SELECT
	            CONTA.[ID],
	            CONTA.[TITULAR],
                CONTA.[DATAABERTURA],
	            CONTA.[DATAFECHAMENTO],
	            CONTA.[MESA_ID],
	            CONTA.[GARCOM_ID],
	            CONTA.[ESTAABERTA],
	            
                GARCOM.[NOME],
                GARCOM.[CPF],
                MESA.[NUMERO],
                MESA.[CAPACIDADE],
                MESA.[ESTAOCUPADA]
            FROM
                [TBCONTA] AS CONTA
            INNER JOIN
                [TBGARCOM] AS GARCOM ON CONTA.[GARCOM_ID] = GARCOM.[ID]
            INNER JOIN 
                [TBMESA] AS MESA ON CONTA.[MESA_ID] = MESA.[ID]
            WHERE
                [ESTAABERTA] = @ESTAABERTA";

            SqlConnection conexaoComBanco = new(connectionString);

            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ESTAABERTA", true);


            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            List<Conta> contas = [];

            while (leitor.Read())
            {
                contas.Add(ConverterParaConta(leitor));
            }
            conexaoComBanco.Close();
            return contas;
        }
        public List<Conta> SelecionarContasFechadas()
        {
            string sqlSelecionarTodos =
           @"SELECT
	            CONTA.[ID],
	            CONTA.[TITULAR],
                CONTA.[DATAABERTURA],
	            CONTA.[DATAFECHAMENTO],
	            CONTA.[MESA_ID],
	            CONTA.[GARCOM_ID],
	            CONTA.[ESTAABERTA],
	            
                GARCOM.[NOME],
                GARCOM.[CPF],
                MESA.[NUMERO],
                MESA.[CAPACIDADE],
                MESA.[ESTAOCUPADA]
            FROM
                [TBCONTA] AS CONTA
            INNER JOIN
                [TBGARCOM] AS GARCOM ON CONTA.[GARCOM_ID] = GARCOM.[ID]
            INNER JOIN 
                [TBMESA] AS MESA ON CONTA.[MESA_ID] = MESA.[ID]
            WHERE
                [ESTAABERTA] = @ESTAABERTA";

            SqlConnection conexaoComBanco = new(connectionString);

            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);
            comandoSelecao.Parameters.AddWithValue("ESTAABERTA", false);


            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            List<Conta> contas = [];

            while (leitor.Read())
            {
                contas.Add(ConverterParaConta(leitor));
            }
            conexaoComBanco.Close();
            return contas;
        }
        public List<Conta> SelecionarContasPorPeriodo(DateTime data)
        {
            const string sqlSelecionarTodos =
            @"SELECT
	            CONTA.[ID],
	            CONTA.[TITULAR],
	            CONTA.[MESA_ID],
	            CONTA.[GARCOM_ID],
	            CONTA.[DATAABERTURA],
	            CONTA.[DATAFECHAMENTO],
	            CONTA.[ESTAABERTA],
                GARCOM.[NOME],
                GARCOM.[CPF],
                MESA.[NUMERO],
                MESA.[CAPACIDADE],
                MESA.[ESTAOCUPADA]
            FROM
                [TBCONTA] AS CONTA
            INNER JOIN
                [TBGARCOM] AS GARCOM ON CONTA.[GARCOM_ID] = GARCOM.[ID]
            INNER JOIN 
                [TBMESA] AS MESA ON CONTA.[MESA_ID] = MESA.[ID]
            WHERE
                CAST([DATAFECHAMENTO] AS DATE) = @DATAFECHAMENTO";

            SqlConnection conexaoComBanco = new(connectionString);

            conexaoComBanco.Open();

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("DATAFECHAMENTO", data.Date);

            SqlDataReader leitor = comandoSelecao.ExecuteReader();

            List<Conta> contas = [];

            while (leitor.Read())
            {
                contas.Add(ConverterParaConta(leitor));
            }

            conexaoComBanco.Close();

            return contas;
        }
        public void ConfigurarParametrosConta(Conta conta, SqlCommand comandoInsercao)
        {
            comandoInsercao.Parameters.AddWithValue("ID", conta.Id);
            comandoInsercao.Parameters.AddWithValue("TITULAR", conta.Titular);
            comandoInsercao.Parameters.AddWithValue("MESA_ID", conta.Mesa.Id);
            comandoInsercao.Parameters.AddWithValue("GARCOM_ID", conta.Garcom.Id);
            comandoInsercao.Parameters.AddWithValue("DATAABERTURA", conta.Abertura == default ? DBNull.Value : (object)conta.Abertura);
            comandoInsercao.Parameters.AddWithValue("DATAFECHAMENTO", conta.Fechamento == default ? DBNull.Value : (object)conta.Fechamento);
            comandoInsercao.Parameters.AddWithValue("ESTAABERTA", conta.EstaAberta);    
        }
        public void AdicionarPedido(Conta conta, Pedido pedido)
        {
            var sqlInserirPedido =
                @"INSERT INTO [TBPEDIDO]
                    (
                        [ID],
                        [PRODUTO_ID],
                        [QUANTIDADE],
                        [VALOR]
                    )
                VALUES
                    (
                        @ID,
                        @PRODUTO_ID,
                        @QUANTIDADE,
                        @VALOR
                    )";
            SqlConnection conexaComBanco = new SqlConnection(connectionString);
            SqlCommand comandoInsercao = new SqlCommand(sqlInserirPedido, conexaComBanco);
         
            ConfigurarParametrosPedido(pedido, comandoInsercao);

            conexaComBanco.Open();

            comandoInsercao.ExecuteNonQuery();
            conta.Pedidos.Add(pedido);
            conexaComBanco.Close();
        }
        public void ConfigurarParametrosPedido(Pedido pedido, SqlCommand comandoInsercao)
        {
            
            comandoInsercao.Parameters.AddWithValue("ID", pedido.Id);
            comandoInsercao.Parameters.AddWithValue("PRODUTO_ID", pedido.Produto.Id);
            comandoInsercao.Parameters.AddWithValue("QUANTIDADE", pedido.QuantidadeSolicitada);
            comandoInsercao.Parameters.AddWithValue("VALOR", pedido.Produto.Valor * pedido.QuantidadeSolicitada);
        }
    }
}
    