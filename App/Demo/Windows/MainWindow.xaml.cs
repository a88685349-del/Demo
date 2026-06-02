using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Demo
{
    public partial class MainWindow : Window
    {
        Image[] slots = new Image[4];
        int attempts = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitCaptcha();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            bool captchaOk = true;
            for (int i = 0; i < 4; i++)
                if ((int)slots[i].Tag != i + 1) captchaOk = false;

            if (!captchaOk)
            {
                MessageBox.Show("Неверно пройдена капча! Попробуйте ещё раз.");
                InitCaptcha();
                return;
            }

            if (TbLogin.Text == "" || PbPass.Password == "")
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = user182_dbEntities.GetContext();
            var user = db.Demo_Polzovateli.FirstOrDefault(p => p.login_polzovatelya == TbLogin.Text && p.parol == PbPass.Password);

            if (user != null)
            {
                if (user.zablokirovan == true)
                {
                    MessageBox.Show("Вы заблокированы!");
                    return;
                }

                MessageBox.Show("Вы авторизованы!");

                if (user.id_roli == 1) new AdminWindow().Show();
                else new UserWindow().Show();

                Close();
            }
            else
            {
                attempts++;

                if (attempts >= 3)
                {
                    var checkuser = db.Demo_Polzovateli.FirstOrDefault(p => p.login_polzovatelya == TbLogin.Text);
                    if (checkuser != null)
                    {
                        checkuser.zablokirovan = true;
                        db.SaveChanges();
                    }
                    MessageBox.Show("Вы заблокированы!");
                }
                else
                {
                    MessageBox.Show("Вы ввели неверный логин или пароль!");
                }
            }
        }

        private Image MakeSlot(int num)
        {
            var img = new Image
            {
                Source = new BitmapImage(new Uri($"/Resources/{num}.png", UriKind.Relative)),
                Width = 125,
                AllowDrop = true,
                Tag = num
            };

            img.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragDrop.DoDragDrop(img, img.Tag, DragDropEffects.Move);
            };

            img.Drop += (s, e) =>
            {
                int fromNum = (int)e.Data.GetData(typeof(int));
                var fromSlot = slots.First(x => (int)x.Tag == fromNum);

                var tempTag = fromSlot.Tag;
                fromSlot.Tag = img.Tag;
                img.Tag = tempTag;

                var tempSource = fromSlot.Source;
                fromSlot.Source = img.Source;
                img.Source = tempSource;
            };

            return img;
        }

        private void InitCaptcha()
        {
            GridPanel.Children.Clear();
            var rnd = new Random();
            var nums = Enumerable.Range(1, 4).OrderBy(x => rnd.Next()).ToArray();

            for (int i = 0; i < 4; i++)
            {
                slots[i] = MakeSlot(nums[i]);
                GridPanel.Children.Add(slots[i]);
            }
        }
    }
}