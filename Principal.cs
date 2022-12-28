using System.IO;
using System;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;




namespace Projeto_Nota_Fiscal
{

    public partial class NFE : Form
    {
        string linha;
        string[] Prencher;

        List<Lista> lstLista = new List<Lista>();

        public NFE()
        {
            InitializeComponent();
            btnConfirmar.Enabled = false;   
        }

        public class Lista
        {
            public string numeroDocumento { get; set; }
            public string Parcela { get; set; }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            lstLista.Clear();   
            Arquivo();
           
        }

        private void Arquivo()
        {

            dataGridView1.DataSource = null;
            OpenFileDialog paste = new OpenFileDialog();


            paste.Filter = "All files (*.csv)|*.csv";
            paste.Title = "Selecione o arquivo";
            paste.ShowDialog();

            string csv = paste.FileName;

            StreamReader lerArquivo = new StreamReader(File.OpenRead(csv));


            while ((linha = lerArquivo.ReadLine()) != null)
            {

                Prencher = linha.Split(';');

                if (Prencher.Count() == 2)
                {
                    

                    Lista lista1 = new Lista();

                    lista1.numeroDocumento = Prencher[0].ToString();
                    lista1.Parcela = Prencher[1].ToString();
                    if (lista1.Parcela != "0")
                    {
                        if (lista1.Parcela.Count() < 2)
                        {
                            lista1.Parcela = ("0" + Prencher[1].ToString());
                        }
                      
                        lstLista.Add(lista1);
                        
                    }
                    else
                    {
                        MessageBox.Show("Arquivos contem Registros Inconsistentes");

                    }
                }
            }
            dataGridView1.DataSource = lstLista;
            btnConfirmar.Enabled = true;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult sair = new DialogResult();
            sair = MessageBox.Show("Deseja Sair?", "Alerta", MessageBoxButtons.YesNo);
            if (sair == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            int controle = 0;
            foreach (var item in lstLista)
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.portaldocliente.online/api/data-integration/v1/app/liz/company/61558037000124/account-receivable/{item.numeroDocumento}/quota/{item.Parcela}");
                request.Method = "DELETE";
                request.ContentType = "application/json";
                request.Headers.Add("x-api-key", "6ZIZ2piACjayzQJb55GUQ7TIRsfrrgY5zi46G6g1");

                string json = JsonConvert.SerializeObject(item);

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();

                    try
                    {
                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            //MessageBox.Show(item.numeroDocumento + " Processado com Exito");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(item.numeroDocumento + " não encontrado ou já processado !!");
                        controle = 1;

                    }

                }
            }

            if (controle == 0)
            {
                MessageBox.Show("Arquivo Processado com Exito");
            }
        }
    }
}



