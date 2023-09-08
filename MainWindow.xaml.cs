using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



// Heatmapの表示
//
// ヒートマップ:https://scottplot.net/cookbook/4.1/#heatmap
// カラーバー:  https://scottplot.net/cookbook/4.1/category/plottable-colorbar/
// カラーマップ:  https://scottplot.net/cookbook/4.1/colormaps/
//


namespace ColorBarRGB
{

    // 
    public class RGBData
    {
        public byte index { get; set; }   //  0-255

        public byte r_val { get; set; }   // R値( 0 - 255 )

        public byte g_val { get; set; }   // G値 ( 0 - 255 )

        public byte b_val { get; set;}    // B値 ( 0 - 255 )
    }



    public partial class MainWindow : Window
    {
        public static ObservableCollection<RGBData> rgb_list;     // RGB値 
        
        Colormap colormap;

       

        public MainWindow()
        {
            InitializeComponent();

            rgb_list = new ObservableCollection<RGBData>();         //  クラス RGBDataのコレクションをデータバインディングするため、ObservableCollectionで生成
            this.RGB_DataGrid.ItemsSource = rgb_list;               //  データグリッド ( RGB_DataGrid )のソース指定

            wpfPlot_Colorbar.Configuration.Pan = false;   // パン(グラフの移動)不可
            wpfPlot_Colorbar.Configuration.ScrollWheelZoom = false; //ズーム(グラフの拡大、縮小)不可

            RB_Viridis.IsChecked = true;            // カラーマップ Viridis 選択
            RB_Turbo.IsChecked = false;
            RB_Blues.IsChecked = false; 
            RB_Grayscale.IsChecked = false; 

            Scale_Auto_CheckBox.IsChecked = true;   // カラーマップ　上下限値は、表示データの最大最小値

            Interpolation_CheckBox.IsChecked = false;  // Smooth(interpolation) なし

            Axis_CheckBox.IsChecked = true;            // 軸の目盛り表示あり

            DataRC_CheckBox.IsChecked = true;         // データ表示あり

        }



        // カラーマップ、カラーバーの表示
        private void Disp_Color_Map_Bar()
        {
            double[,] data2D;

            data2D = new double[2, 3];

            data2D[0, 0] = 1;
            data2D[0, 1] = 2;
            data2D[0, 2] = 3;
            data2D[1, 0] = 4;
            data2D[1, 1] = 5;
            data2D[1, 2] = 6;


            wpfPlot_Colorbar.Plot.Clear(); // クリア

        

            var hm = wpfPlot_Colorbar.Plot.AddHeatmap(data2D, colormap, lockScales: false); // ヒートマップ表示  data2D[col,row]を左上から順に表示

            var cb = wpfPlot_Colorbar.Plot.AddColorbar(hm); // カラーバー表示


            if (Axis_CheckBox.IsChecked == false ) {      // 軸の目盛り表示なし
                wpfPlot_Colorbar.Plot.XAxis.Ticks(false, false, false); // X軸目盛り(大、小)とラベルの非表示
                wpfPlot_Colorbar.Plot.YAxis.Ticks(false, false, false); // Y軸目盛り(大、小)とラベルの非表示
            }
            else
            {
                wpfPlot_Colorbar.Plot.XAxis.Ticks(true, true, true); // X軸目盛り(大、小)とラベルの非表示
                wpfPlot_Colorbar.Plot.YAxis.Ticks(true, true, true); // Y軸目盛り(大、小)とラベルの非表示
            }


            if (Scale_Auto_CheckBox.IsChecked == false) {    // Autoスケールでない場合
                double.TryParse(TB_ColorBar_Max.Text, out double max_val);  // 最大値の設定
                double.TryParse(TB_ColorBar_Min.Text, out double min_val);  // 最小値の設定

                hm.Update(data2D, min: min_val, max: max_val);
            }


            if (Interpolation_CheckBox.IsChecked == true) {
                hm.Smooth = true;               // 補間あり
            }
            
            if ( DataRC_CheckBox.IsChecked == true)
            {
                for (int i = 0; i < 2; i++)       //   配列データの表示
                {
                    for (int j = 0; j < 3; j++)
                    {
                    string str_data = "data2D[" + i.ToString() + "," + j.ToString() + "] = " +  data2D[i, j].ToString();
                    
                    wpfPlot_Colorbar.Plot.AddText(str_data, 0.2 + j, 1.5 - i, size:10, color: System.Drawing.Color.White);
                    }
                }
            }

            wpfPlot_Colorbar.Render();      // ヒートマップの表示       
        
            rgb_list.Clear();               // Listのクリア

            for (int i = 0; i < 256; i++)
            {
                byte pt = (byte)i;

                (byte r, byte g, byte b) = colormap.GetRGB(pt); // RGB値を得る

                RGBData rgbdata = new RGBData();

                rgbdata.index = pt;

                rgbdata.r_val = r;
                rgbdata.g_val = g;
                rgbdata.b_val = b;

                rgb_list.Add(rgbdata);          // Listへ追加
            }
        }


        // ラジオボタンが押された時の処理　
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (RB_Viridis.IsChecked == true)
            {
                colormap = ScottPlot.Drawing.Colormap.Viridis;   // カラーマップ Viridis
            }

            else if (RB_Turbo.IsChecked == true)
            {
                    colormap = ScottPlot.Drawing.Colormap.Turbo; // カラーマップ Turbo
            }

            else if (RB_Blues.IsChecked == true)
            {
                    colormap = ScottPlot.Drawing.Colormap.Blues; // カラーマップ Blues
            }
            else if (RB_Grayscale.IsChecked == true)
            {
                    colormap = ScottPlot.Drawing.Colormap.Grayscale;  // カラーマップ Grayscale
             }
       

        }

        // 表示の更新
        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {

            Disp_Color_Map_Bar();       // カラーマップ、カラーバーの表示

        }


        // RGBデータの保存
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string path;

            string str_rgb_line;


            SaveFileDialog sfd = new SaveFileDialog();           //　SaveFileDialogクラスのインスタンスを作成 

            sfd.FileName = "colorbar_rgb.txt";                         //「ファイル名」で表示される文字列を指定する

            sfd.Title = "保存先のファイルを選択してください。";        //タイトルを設定する 

            sfd.RestoreDirectory = true;                 //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

            if (sfd.ShowDialog() == true)            //ダイアログを表示する
            {
                path = sfd.FileName;

                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);

                    str_rgb_line = "";

                    int cnt = 0;
                    // { 48,18,59},
                    foreach (RGBData rgbData in rgb_list)         // rgb_listの内容を保存
                    {
                        
                        string st_r = rgbData.r_val.ToString();   // R
                        string st_g = rgbData.g_val.ToString();   // G
                        string st_b = rgbData.b_val.ToString();   // B


                        str_rgb_line = str_rgb_line + "{" + st_r + "," + st_g + "," + st_b + "}," ;
                        cnt++;

                        if ( cnt % 16  == 0 ) {
                            str_rgb_line = str_rgb_line + "\r\n";
                        }


                    }

                    sw.WriteLine(str_rgb_line);         // 1行保存

                    sw.Close();
                }

                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        
        // Auto　チェック時の処理
        // カラーマップのスケール自動の場合、
        // 表示データの最小値と最大値が、それぞれカラーマップの両端の色となる。
        private void Scale_Checked(object sender, RoutedEventArgs e)
        {
            TB_ColorBar_Min.IsEnabled = false;
            TB_ColorBar_Max.IsEnabled = false;  
        }

        // Auto 未チェック時の処理
        private void Scale_Unchecked(object sender, RoutedEventArgs e)
        {
            TB_ColorBar_Min.IsEnabled = true;
            TB_ColorBar_Max.IsEnabled = true;
        }
    }
}
