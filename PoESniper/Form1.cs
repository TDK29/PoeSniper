using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PoESniper
{
    public partial class Form1 : Form
    {
        private PoEAPI myPoEAPI;
        private int count = 0;
        public Form1()
        {
            InitializeComponent();
            myPoEAPI = new PoEAPI();
            labelLastUpdate.Text = "Last Update:" +  myPoEAPI.getNextChangeID();
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerUpdate.IsBusy)
            { 
            
                backgroundWorkerUpdate.CancelAsync();
                buttonStartStop.Text = "Start";
            }
            else
            {
                myPoEAPI.updateNextChangeID();
                backgroundWorkerUpdate.RunWorkerAsync();
                buttonStartStop.Text = "Stopp";
                labelSync.Text = "Fetching First Data. This may take a while";
            }
        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.backgroundWorkerUpdate.CancellationPending)
            {
                DateTime starttime = System.DateTime.Now;
                JObject response = myPoEAPI.doUpdate();
                backgroundWorkerUpdate.ReportProgress(1, myPoEAPI.getNextChangeID());
                //labelLastUpdate.Text = "Last Update:" + myPoEAPI.getNextChangeID();
                backgroundWorkerUpdate.ReportProgress(4, response);
                if(System.DateTime.Compare(starttime.AddMilliseconds(3000),System.DateTime.Now)>0)
                {
                    backgroundWorkerUpdate.ReportProgress(2);
                    //labelSync.Text = "In Sync";
                    while (System.DateTime.Compare(starttime.AddMilliseconds(1000), System.DateTime.Now)>0)
                        Thread.Sleep(100);
                }else
                {
                    backgroundWorkerUpdate.ReportProgress(3);
                    //labelSync.Text = "Out of Sync! Results may be slow";
                }
            }
            return;
        }

        private void backgroundWorkerUpdate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 1:
                    labelLastUpdate.Text = "Last Update: " + e.UserState;
                    break;
                case 2:
                    Random rand = new Random();
                    labelSync.Text = "In Sync " + rand.Next(0,32500);
                    break;
                case 3:
                    labelSync.Text = "Out of Sync! Results may be slow";
                    break;
                case 4:
                    JObject results = e.UserState as JObject;
                    IList<JToken> stashes = results["stashes"].Children().ToList();

                    foreach (JToken stash in stashes)
                    {
                        IList<JToken> items = stash["items"].Children().ToList();
                        foreach (JToken item in items)
                        {
                            count++;
                            foreach (ListViewItem lv in listViewItems.Items)
                            {
                                string test = item["name"].ToString();
                                string test2 = item["typeLine"].ToString();
                                string test3 = test + " " + test2;
                                if (test3.Contains(lv.Text)&& item["league"].ToString().Equals(lv.SubItems[1].Text))
                                {
                                    try
                                    {
                                        ListViewItem viewItem = new ListViewItem(lv.Text);
                                        viewItem.SubItems.Add(stash["lastCharacterName"].ToString());
                                        try
                                        {
                                            viewItem.SubItems.Add(item["note"].ToString());
                                            string whisperText = "@" + stash["lastCharacterName"].ToString() + " Hi, I'd like to buy your " + test3 + " listed for " + item["note"].ToString() + " in " + item["league"].ToString() + " (stash tab\"" + stash["stash"].ToString() + "\"; position: left " + item["x"].ToString() + ", top " + item["y"].ToString() + ")";
                                            viewItem.ToolTipText = whisperText;
                                        }
                                        catch (Exception e2)
                                        {
                                            viewItem.SubItems.Add("No Buyout Price");
                                        }
                                       // viewItem.SubItems.Add("@" + stash["lastCharacterName"].ToString() +
                                       //     " Hi, I'd like to buy your " + test3 + " listed for " + item["note"].ToString()
                                       //     + "in" + item["league"].ToString() + "(stash tab\"" + stash["stash"].ToString() + "\"; position: left " +
                                       //     item["x"].ToString() + ", top " + item["y"].ToString() + ")");
                                        listViewResults.Items.Add(viewItem);
                                        listViewResults.EnsureVisible(listViewResults.Items.Count - 1);
                                    }catch (Exception e1)
                                    {
                                    }
                                    
                                }
                            }
                        }
                    }
                    labelDebug.Text = "Processed entries: " + count;
                    break;
            }
        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormAdd())
            {
                var result = form.ShowDialog();
                if(result == DialogResult.OK)
                {
                    ListViewItem item = new ListViewItem(form.name);
                    item.SubItems.Add(form.league);
                    item.SubItems.Add(form.price);
                    listViewItems.Items.Add(item);
                }
            }
        }

        private void listViewItems_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                foreach(ListViewItem item in listViewItems.SelectedItems)
                {
                    item.Remove();
                }
            }
        }

        private void listViewResults_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.C && e.Control)
            {
                Clipboard.SetText(listViewResults.SelectedItems[0].ToolTipText);
            }
        }
    }
}
