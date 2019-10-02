using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;
using TravelExpertsDB;
using Travel_Experts_Services_WPF.Properties;

namespace Travel_Experts_Services_WPF
{
    public partial class MainWindow : Window
    {
        // create constants
        const int PKG_NAME_LENGTH = 50;
        const int PKG_DESC_LENGTH = 50;
        const double PRICE_MAX = 922337203685477.5807;
        // variables
        int pkgid;
        int slct_colmn = -1;
        // display products & suppliers
        List<Products> Prod = null;
        List<Products> selectProducts;
        List<Supplier> Sup = null;
        List<Supplier> selectSuppliers;
        List<PackageProductSuppliers> ppss = new List<PackageProductSuppliers>();
        List<ProdSuppliersNames> psn = new List<ProdSuppliersNames>();
        List<Packages> PackagesList;

        List<Products> Prodd = ProductDB.GetProducts();
        List<Supplier> Supp = SupplierDB.GetSuppliers();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void GvPackages_Loaded(object sender, RoutedEventArgs e)
        {
            //check = 1;

        }


        //Ethan Shipley & Sheila Zhao
        // On form load performs these actions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load the username and password if the user checked RememberMe checkbox before
            if (Settings.Default.UserName != "")
            {
                txtUserName.Text = Settings.Default.UserName;
                txtPassword.Password = Settings.Default.Password;
                ckbRemember.IsChecked=true;
            }
            else
            {
                txtUserName.Focus();
            }

            PackagesGrid();
            ProductList();
            SupplierList();
            gvProdSup_pkg.ItemsSource = "";
            gvProdSup_pkg.ItemsSource = ppss;
        }

        //Ethan Shipley January 28 2019
        //Sets the data source and formatting for the packages datagridview
        private void PackagesGrid()
        {
            PackagesList = PackagesDB.GetPackages();
            gvPackages.ItemsSource = PackagesList;
            gvPackages.Focus();

        }

        //Ethan SHipley January 28 2019
        //sets the datasource and formatting for the products datagridview
        private void ProductsGrid()
        {
            gvProducts.ItemsSource = ppss;
        }

        //Ethan SHipley January 28 2019
        //sets the datasource and formatting for the products datagridview
        private void SuppliersGrid()
        {
            gvSuppliers.ItemsSource = ppss;
        }



        //Ethan Shipley
        // Hides and unhides prod suppplier gridview and button
        private void hideunhide(bool hd)
        {
            // if hd is true then is shows the following fields
            if (hd)
            {
                btnAddPkgProdSup.Visibility = Visibility.Visible;
                gvProdSup_all_pkgs.Visibility = Visibility.Visible;
            }
            // else if hd is false then the following are hidden
            else
            {
                btnAddPkgProdSup.Visibility = Visibility.Hidden;
                gvProdSup_all_pkgs.Visibility = Visibility.Hidden;
            }
        }

        // Ethan Shipley
        // Sets the text boxes and user inputs on the packages tab to be cleared
        private void BtnAddPkg_Click(object sender, RoutedEventArgs e)
        {
            // changes the text for the add/edit button on the packages pages and changes to the packages page
            tabControl1.SelectedIndex = 2;
            PackagesReset();
            slct_colmn = -1;
        }

        //Ethan Shipley
        // Resets the data fields on the packages page
        public void PackagesReset()
        {
            // changes the text for the add/edit button on the packages pages and changes to the packages page
            txtPackageName.Text = "";
            dtpPkgStartDate.Text = Convert.ToString(DateTime.Now);
            dtpPkgEndDate.Text = Convert.ToString(DateTime.Now); ;
            txtPkgDesc.Text = "";
            txtPkgBasePrice.Text = "";
            txtPkgAgencyCommission.Text = "";
            
            // clears the data for the gridviews
            gvProdSup_pkg.ItemsSource = "";
            gvProdSup_all_pkgs.ItemsSource = "";
            
            //hides the button and datagrid
            hideunhide(false);
            AddEditPkgBtn();
        }

        //Ethan Shipley
        // Gets the cells value from selected cell based on required column
        private int getSelectedCellValue(DataGrid dataGridView)
        {
            object item = dataGridView.SelectedItem;
            return Convert.ToInt32(item.GetType().GetProperty("PackageID").GetValue(item, null));
        }

        //Ethan Shipley February 4, 2019
        // Depending on whether the person is adding or editing a packages the button is changed size and writing
        private void AddEditPkgBtn(string content = "Add")
        {
            // If user has added a new package
            if (content=="Add")
            {
                btnAddEditPkg.Content = "Save New Package";
                btnAddEditPkg.Width = 140;
                btnAddEditPkg.Margin = new Thickness(10, 355, 894, 0);
            }
            //if user has edited a package
            else if (content=="Edit")
            {
                btnAddEditPkg.Content = "Save Edited Package";
                btnAddEditPkg.Width = 152;
                btnAddEditPkg.Margin = new Thickness(10, 355, 882, 0);
            }
        }

        // Ethan Shipley
        // When user selects a package to edit it is then filled out automatically in the packages tab
        private void BtnEditPkg_Click(object sender, EventArgs e)
        {
            if (gvPackages.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a package.");
                return;
            }
            // changes the text for the add/edit button on the packages pages and changes to the packages page
            tabControl1.SelectedIndex = 2;
            AddEditPkgBtn("Edit");

            //Grabs the selected package from the packages gridview
            var Packages = from Pkg in PackagesList
                           where Pkg.PackageID == getSelectedCellValue(gvPackages)
                           select new
                           {
                               Pkg.PackageID,
                               Pkg.PkgName,
                               Pkg.PkgStartDate,
                               Pkg.PkgEndDate,
                               Pkg.PkgDesc,
                               Pkg.PkgBasePrice,
                               Pkg.PkgAgencyCommission
                           };
            //once the package has been grabbed the packages page is filled out with all the fields to be changed
            foreach (var item in Packages)
            {
                pkgid = item.PackageID;
                txtPackageName.Text = item.PkgName;
                dtpPkgStartDate.Text = item.PkgStartDate.ToString();
                dtpPkgEndDate.Text = item.PkgEndDate.ToString();
                txtPkgDesc.Text = item.PkgDesc;
                txtPkgBasePrice.Text = item.PkgBasePrice.ToString("c");
                txtPkgAgencyCommission.Text = item.PkgAgencyCommission.ToString("c");
            }
            // updates the packages gridview and selects edited package then hides products_supplier packages gridview and add button
            UpdateBinding(true);
            hideunhide(false);
        }

        // Ethan Shipley
        // Updates the datasource for grid views. if input is true then there is already an existing selected row
        // if false then new row is what is wanted to be selected
        private void UpdateBinding(bool crnt_cell)
        {
            //
            PackagesList = PackagesDB.GetPackages();
            // if true an existing row is what is needed to be selected in packages gridview
            if (crnt_cell)
            {
                slct_colmn = gvPackages.SelectedIndex;
                PackagesGrid();
                gvPackages.SelectedIndex = slct_colmn;
                gvPackages.Focus();
            }
            //if false a new row is being created and is what is needed to be slected
            else
            {
                int indx = gvPackages.Items.Count + 0;
                PackagesGrid();
                gvPackages.SelectedIndex = indx;
                gvPackages.Focus();
                slct_colmn = indx;
            }

            // sets the itemsources for individual datagrids
            gvProdSup_pkg.ItemsSource = ppss;
            gvProdSup_all_pkgs.ItemsSource = psn;

            // sets the itemsources for products and suppliers datagrids
            gvProducts1.ItemsSource = Prodd;
            gvSuppliers2.ItemsSource = Supp;

        }

        //Ethan Shipley
        // Deletes a selected package
        private void BtnDeletepkg_Click(object sender, RoutedEventArgs e)
        {
            // Checks to see if a package has been selected, if no package selected then no package will be deleted
            if (gvPackages.SelectedIndex==-1)
            {
                MessageBox.Show("Cannot delete package. Please select a package to delete.");
                return;
            }
            // confirms that the user wants to delete the package
            if (!deleteConfirm())
            {
                return;
            }
            //when user has confirmed deletion then the packages is deleted and data is reloaded
            PackagesDB.DeletePackage(pkgid);
            PackagesGrid();
            PackagesReset();
        }

        //Ethan Shipley
        // Confirms that the user wants to delete
        private bool deleteConfirm()
        {
            // Displays message asking user if they are sure that they want to delete the item
            var confirm = MessageBox.Show("Do you want to delete the item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                // confirms again that the user wants to perform the deletion
                confirm = MessageBox.Show("Do you really want to delete the item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Ethan Shipley & Sheila Zhao
        // Gets the products and suppliers when a package is selected
        private void GvPackages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Packages selectedPackage = null;
            try
            {
                object row = gvPackages.SelectedItem;
                selectedPackage = new Packages(
                        Convert.ToInt32(row.GetType().GetProperty("PackageID").GetValue(row, null)),
                        row.GetType().GetProperty("PkgName").GetValue(row, null).ToString(),
                        (DateTime?)(row.GetType().GetProperty("PkgStartDate").GetValue(row, null)),
                        (DateTime?)(row.GetType().GetProperty("PkgEndDate").GetValue(row, null)),
                        row.GetType().GetProperty("PkgDesc").GetValue(row, null).ToString(),
                        Convert.ToDecimal(row.GetType().GetProperty("PkgBasePrice").GetValue(row, null)),
                        (decimal)(row.GetType().GetProperty("PkgAgencyCommission").GetValue(row, null)));
                ppss = PackageProductSuppliersDB.GetProductSuppliersByPackage(selectedPackage);
                pkgid = selectedPackage.PackageID;
                ProductsGrid();
                SuppliersGrid();
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        /*=======================================================================================================================*/
        /*=======================================================================================================================*/
        /*=================================================TitleBar & Login======================================================*/
        /*=======================================================================================================================*/
        /*=======================================================================================================================*/

        // Sheila Zhao
        // set the title bar grid to be able to drag move
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Sheila Zhao
        // minimize the application when click 
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        // Sheila Zhao
        // set the title bar grid to be able to drag move
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Sheila Zhao
        // exit the application when click
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Sheila Zhao
        //login to the application
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {   
            // username can not be empty
            if (txtUserName.Text == "")
            {
                MessageBox.Show("Please enter user name", "Error", MessageBoxButton.OK);
                txtUserName.Focus();
                return;
            }
            // password can not be empty
            if (txtPassword.Password == "")
            {
                MessageBox.Show("Please enter password", "Error", MessageBoxButton.OK);
                txtPassword.Focus();
                return;
            }
            try
            {
                // define connection
                SqlConnection con = DBConnection.GetConnection();                
                // define the select query command
                String query = "SELECT Username, Password FROM Users " +
                                "WHERE Username = @UserName " +
                                "AND Password = @Password";
                
                SqlCommand cmd = new SqlCommand(query, con);

                SqlParameter uname = new SqlParameter("@UserName", SqlDbType.VarChar);
                SqlParameter upassword = new SqlParameter("@Password", SqlDbType.VarChar);
                // get the input
                uname.Value = txtUserName.Text;
                upassword.Value = txtPassword.Password;
                
                cmd.Parameters.Add(uname);
                cmd.Parameters.Add(upassword);
                // open the connection
                cmd.Connection.Open();
                // execute the query
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                // if the user's username and password is correct, user gets access to other functions
                if (myReader.Read() == true)
                {
                    // if user checks the remember me checkbox, username and password will be saved
                    if (ckbRemember.IsChecked ?? true)
                    {
                        Settings.Default.UserName = txtUserName.Text;
                        Settings.Default.Password = txtPassword.Password;
                        Settings.Default.Save();
                    }
                    // display other tabs
                    tbiPackages.Visibility = Visibility.Visible;
                    tbiPkgOverview.Visibility = Visibility.Visible;
                    tbiProducts.Visibility = Visibility.Visible;
                    tbiSuppliers.Visibility = Visibility.Visible;
                    lblMain.Text = "Welcome Travel Experts";
                    tbiMainPage.Header = "Home";

                    // hide the login part
                    txtUserName.Visibility = Visibility.Hidden;
                    txtPassword.Visibility = Visibility.Hidden;
                    lblUserName.Visibility = Visibility.Hidden;
                    lblPassword.Visibility = Visibility.Hidden;
                    btnLogin.Visibility = Visibility.Hidden;
                    btnReset.Visibility = Visibility.Hidden;
                    ckbRemember.Visibility = Visibility.Hidden;
                    HomeImage.Visibility = Visibility.Visible;

                }
                else
                {
                    MessageBox.Show("Login Failed... Try again!", "Login Denied", MessageBoxButton.OK);

                    txtUserName.Clear();
                    txtPassword.Clear();
                    txtUserName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        // save user's infomation when the checkbox is checked
        private void CkbRemember_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.UserName = txtUserName.Text;
            Settings.Default.Password = txtPassword.Password;
            Settings.Default.Save();
        }

        // clear user's information when the checkbox is unchecked
        private void CkbRemember_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
        }

        // clear textboxes and clear user's information when click reset
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtUserName.Clear();
            txtPassword.Clear();
            txtUserName.Focus();
            ckbRemember.IsChecked = false;
            Settings.Default.Reset();
        }

        /*=======================================================================================================================*/
        /*=======================================================================================================================*/
        /*===================================================PACKAGES============================================================*/
        /*=======================================================================================================================*/
        /*=======================================================================================================================*/

        // Ethan Shipley
        // creates or edits a package based and verifies the correct input of data
        private void BtnAddEditPkg_Click(object sender, RoutedEventArgs e)
        {
            // declare variables
            Packages pack = new Packages();
            DateTime? strt = dtpPkgStartDate.SelectedDate;
            DateTime? end = dtpPkgEndDate.SelectedDate;
            if (txtPackageName.Text.Length > 50)
            {
                MessageBox.Show("Please enter a package name shorter then 50 characters.");
                return;
            }
            else
            {
                pack.PkgName = txtPackageName.Text.ToString();
            }

            // verifies that the user entered a package description shorter then 50 characters
            if (txtPkgDesc.Text.Length > 50)
            {
                MessageBox.Show("Please enter a package description shorter then 50 characters.");
                return;
            }
            else
            {
                pack.PkgDesc = txtPkgDesc.Text.ToString();
            }

            // checks that the user inputted valid dates for the package
            if (end < strt && !end.Value.Date.ToShortDateString().Equals(strt.Value.Date.ToShortDateString()))
            {
                dtpPkgEndDate.SelectedDate = dtpPkgStartDate.SelectedDate;
                MessageBox.Show("Please enter an end date that is equal to or greater than the start date.");
                return;
            }
            else if (strt < DateTime.Now && !DateTime.Now.ToShortDateString().Equals(dtpPkgEndDate.SelectedDate.Value.Date.ToShortDateString()))
            {

                MessageBox.Show("Please enter a start date that is greater than todays date.");
                dtpPkgStartDate.SelectedDate = DateTime.Now;
                return;
            }
            else if (end < DateTime.Now && !DateTime.Now.ToShortDateString().Equals(dtpPkgEndDate.SelectedDate.Value.Date.ToShortDateString()))
            {
                MessageBox.Show("Please enter an end date that is greater than todays date.");
                dtpPkgEndDate.SelectedDate = DateTime.Now;
                return;
            }
            else
            {
                pack.PkgStartDate = Convert.ToDateTime(strt.Value.Date.ToShortDateString());
                pack.PkgEndDate = Convert.ToDateTime(end.Value.Date.ToShortDateString());
            }
            // removes the unnecessary dollar sign and comma in the base price and commision
            decimal PkgBasePrice;
            decimal PkgAgencyCommission;
            try
            {
                PkgBasePrice = Convert.ToDecimal(txtPkgBasePrice.Text.ToString().Replace("$", "").Replace(",", ""));
                if (PkgBasePrice <= 0)
                {
                    MessageBox.Show("Please enter a number for base price greater than zero.");
                    txtPkgBasePrice.Text = "";
                    return;
                }
                if ((double)PkgBasePrice > PRICE_MAX)
                {
                    MessageBox.Show("Please enter a number for base price less than 922,337,203,685,477.5807.");
                    txtPkgBasePrice.Text = "";
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a number for base price.");
                txtPkgBasePrice.Text = "";
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Please enter a number for base price below 7.9228 x 102^8.");
                txtPkgBasePrice.Text = "";
                return;
            }
            pack.PkgBasePrice = PkgBasePrice;
            try
            {
                PkgAgencyCommission = Convert.ToDecimal(txtPkgAgencyCommission.Text.ToString().Replace("$", "").Replace(",", ""));
                if (PkgAgencyCommission <= 0)
                {
                    MessageBox.Show("Please enter a number for agency commision greater than zero.");
                    txtPkgAgencyCommission.Text = "";
                    return;
                }
                if ((double)PkgAgencyCommission > PRICE_MAX)
                {
                    MessageBox.Show("Please enter a number for agency commision less than 922,337,203,685,477.5807.");
                    txtPkgAgencyCommission.Text = "";
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a number for agency commision.");
                txtPkgAgencyCommission.Text = "";
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Please enter a number for agency commision below 7.9228 x 102^8.");
                txtPkgAgencyCommission.Text = "";
                return;
            }
            pack.PkgAgencyCommission = PkgAgencyCommission;


            // Checks the text of the Add/edit package button in order to perform various logic
            if (Convert.ToString(btnAddEditPkg.Content) == "Save New Package")
            {
                //Inserts the package into the database and then refreshes the mainpage
                PackagesDB.InsertPackages(pack);
                UpdateBinding(false);
                AddEditPkgBtn("Edit");
            }
            else if (Convert.ToString(btnAddEditPkg.Content) == "Save Edited Package")
            {
                // updates the package and then refreshes the main page
                PackagesDB.UpdatePackages(pack, pkgid);
                UpdateBinding(true);
            }
        }

        //Ethan Shipley
        // Dsiplays the list of available product suppliers
        private void BtnNewPkgProdSup_Click(object sender, RoutedEventArgs e)
        {
            psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
            gvProdSup_all_pkgs.ItemsSource = psn;
            ProdSupListDetails(gvProdSup_all_pkgs);
            hideunhide(true);
            //ProdSupListDetails(gvProdSup_all_pkgs);
            gvProdSup_all_pkgs.Visibility = Visibility.Visible;
        }

        //Ethan Shipley
        // Formats the prod and supplier details
        private void ProdSupListDetails(DataGrid dataGridView)
        {
            dataGridView.Columns[0].Visibility = Visibility.Hidden;
            dataGridView.Columns[1].Visibility = Visibility.Hidden;
            dataGridView.Columns[2].Visibility = Visibility.Hidden;
            dataGridView.Columns[3].Header = "Product Name";
            dataGridView.Columns[4].Header = "Supplier Name";
        }

        //Ethan Shipley
        // Adds the products suppliers to the package
        private void BtnAddPkgProdSup_Click(object sender, RoutedEventArgs e)
        {
            if (gvProdSup_all_pkgs.SelectedIndex==-1 || slct_colmn==-1)
            {
                MessageBox.Show("Cannot add product. Please add a package or edit an existing package.");
                return;
            }
            PackageProductSuppliersDB.InsertProductSupplierIdPpkg(pkgid, getSelectedCellValueProdSup(gvProdSup_all_pkgs));
            UpdateBinding(true);

            //duplicate code
            psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
            gvProdSup_all_pkgs.ItemsSource = psn;
            ProdSupListDetails(gvProdSup_all_pkgs);
            hideunhide(true);
            gvProdSup_all_pkgs.ItemsSource = psn;
            ProdSupListDetails(gvProdSup_all_pkgs);
            hideunhide(true);
        }

        //Ethan Shipley
        // Deletes the products suppliers to the package
        private void BtnDeletePkgProdSup_Click(object sender, RoutedEventArgs e)
        {
            if (gvProdSup_pkg.SelectedIndex==-1)
            {
                MessageBox.Show("Cannot delete product. Please add a package or edit an existing package.");
                return;
            }
            if (!deleteConfirm())
            {
                return;
            }
            PackageProductSuppliersDB.DeleteProductSupplierIdPpkg(pkgid, getSelectedCellValueProdSup(gvProdSup_pkg));
            slct_colmn = gvPackages.SelectedIndex;

            UpdateBinding(true);

            //duplicate code
            psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
            gvProdSup_all_pkgs.ItemsSource = psn;
            ProdSupListDetails(gvProdSup_all_pkgs);
            hideunhide(false);
        }

        // Ethan Shipley
        // Lets the user know how many cahracters they have left for the package name
        private void TxtPackageName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPkgNameLength.Text = "Remaining length " + Convert.ToString(PKG_NAME_LENGTH - txtPackageName.Text.Length);
        }

        // Ethan Shipley
        // Lets the user know how many cahracters they have left for the package name
        private void TxtPkgDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPkgDescLength.Text = "Remaining length " + Convert.ToString(PKG_DESC_LENGTH - txtPkgDesc.Text.Length);
        }

        //Ethan Shipley
        // Gets the cells value from selected cell based on required column
        private int getSelectedCellValueProdSup(DataGrid dataGridView)
        {
            object item = dataGridView.SelectedItem;
            return Convert.ToInt32(item.GetType().GetProperty("ProductSupplierId").GetValue(item, null));
        }

        //Ethan SHipley
        // CLear the contents of the packages page
        private void BtnClearPkg_Click(object sender, RoutedEventArgs e)
        {
            PackagesReset();
        }
        /*=======================================================================================================================*/
        /*=======================================================================================================================*/
        /*===================================================SUPPLIER============================================================*/
        /*=======================================================================================================================*/
        /*=======================================================================================================================*/



        // Sheila Zhao
        // Gets the supplier information
        private void GetSupplier(int supplierID)
        {
            Sup = SupplierDB.GetSuppliers();
            selectSuppliers = (from Sp in Sup
                               where Sp.SupplierId == supplierID
                               select Sp).ToList();

            gvSuppliers2.ItemsSource = selectSuppliers;
        }

        //Sheila Zhao, with edits by Ethan SHipley
        // Displays all of the suppliers available in the suppliers gridview
        private void SupplierList()
        {
            gvSuppliers2.ItemsSource = Supp;
        }

        //Sheila Zhao
        //Gets the selected supplier and displays the corresponding product when supplier selection changes
        private void GvSuppliers2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Supplier selectedSupplier = null;
            try
            {
                selectedSupplier = (Supplier)GetSelected(gvSuppliers2, "Supplier");
                // display corresponding products with the selected supplier
                Prod = SupplierDB.GetProductsByProductSupplier(selectedSupplier);
                gvProducts2.ItemsSource = Prod;

                if (selectedSupplier != null)
                {
                    // display the product list which the selected supplier doesnt have
                    Prod = ProductDB.GetSupProdNotInList(selectedSupplier);
                    gvProdBySup.ItemsSource = Prod;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        // Sheila Zhao, with edit by Ethan Shipley
        // upon button click of the add new supplier button, the new supplier is saved
        private void BtnNewS_Click(object sender, RoutedEventArgs e)
        {
            Supplier ns = new Supplier();
            btnSaveS.Visibility = Visibility.Hidden;

            try
            {
                if (ns != null)
                {
                    // make sure user cannot create an empty name supplier
                    if (txtSupName.Text == "")
                    {
                        MessageBox.Show("Name can not be empty!");
                    }
                    else
                    {
                        // create and save the new supplier, update the application
                        ns.SupName = txtSupName.Text;
                        SupplierDB.InsertSupplier(ns);
                        Supp = SupplierDB.GetSuppliers();


                        UpdateBindingProdSup();
                        UpdatePNL();
                        UpdateSNL();

                       
                        // make the supplier list to select the last row (new item)
                        ScrollDown(gvSuppliers2);
                    }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
            // clear the textbox
            txtSupName.Text = "";
        }

        // Sheila Zhao, with edits by Ethan SHipley February 1, 2019
        // upon button click for the edit button it gets the selected product and then fills the information into the suppliername textbox
        private void BtnEditS_Click(object sender, RoutedEventArgs e)
        {
            Supplier olds = null;
            btnSaveS.Visibility = Visibility.Visible;
            olds = (Supplier)GetSelected(gvSuppliers2, "Supplier");
            txtSupName.Text = olds.SupName;
            txtSupName.Focus();
        }

        // Sheila Zhao, with edits by Ethan Shipley February 1, 2019
        //Gets the edited supplier for the save function
        private Supplier GetEditSupplier(Supplier olds)
        {
            Supplier news = new Supplier();
            news = (Supplier)GetSelected(gvSuppliers2, "Supplier");
            news.SupName = olds.SupName;
            news.SupplierId = olds.SupplierId;
            return news;
        }

        // Sheila Zhao, with edits by Ethan Shipley February 1, 2019
        // Saves the edited supplier name overwriting the name that was existing
        private void BtnSaveS_Click(object sender, RoutedEventArgs e)
        {
            Supplier news = new Supplier();
            Supplier olds = null;
            olds = (Supplier)GetSelected(gvSuppliers2, "Supplier");
            if (txtSupName.Text == "")
            {
                MessageBox.Show("Name can not be empty!");
            }
            else
            {
                news = GetEditSupplier(olds);
                try
                {
                    news.SupName = txtSupName.Text;
                    SupplierDB.UpdateSupplier(news, olds);
                    Supp = SupplierDB.GetSuppliers();
                    UpdateBindingProdSup();
                    UpdatePNL();
                    UpdateSNL();
                    MessageBox.Show("Supplier Saved!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
                btnSaveS.Visibility = Visibility.Hidden;
                txtSupName.Text = "";
            }
        }

        // Sheila Zhao, with edits by Ethan Shipley February 1 2019
        // Upon button click, the selected supplier is deleted
        private void BtnDelSup_Click(object sender, RoutedEventArgs e)
        {
            btnSaveS.Visibility = Visibility.Hidden;
            try
            {
                Supplier delSup = null;
                delSup = (Supplier)GetSelected(gvSuppliers2, "Supplier");
                if (!deleteConfirm())
                {
                    return;
                }
                SupplierDB.DeleteSupplier(delSup);
                txtSupName.Text = "";
                Supp = SupplierDB.GetSuppliers();
                UpdateBindingProdSup();
                UpdatePNL();
                UpdateSNL();
                ScrollDown(gvSuppliers2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        // Sheila Zhao, with edits by Ethan Shipley February 1 2019
        // Displays the list of products that are not connected to the supplier as well as some buttons
        private void BtnShowSP_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(btnShowSP.Content) == "Show")
            {
                gvProdBySup.Visibility = Visibility.Visible;
                btnDelSP.Visibility = Visibility.Visible;
                btnAddSP.Visibility = Visibility.Visible;
                lblPNL.Visibility = Visibility.Visible;

                btnShowSP.Content = "Hide";
                UpdatePNL();
               
            }
            else if (Convert.ToString(btnShowSP.Content) == "Hide")
            {
                gvProdBySup.Visibility = Visibility.Hidden;
                btnDelSP.Visibility = Visibility.Hidden;
                btnAddSP.Visibility = Visibility.Hidden;
                lblPNL.Visibility = Visibility.Hidden;
                btnShowSP.Content = "Show";

                UpdatePNL();
            }
        }

        private void UpdatePNL()
        {
            Supplier selectedSupplier = null;
            selectedSupplier = (Supplier)GetSelected(gvSuppliers2, "Supplier");
            // update the new list for both on list and not on list
            Prod = SupplierDB.GetProductsByProductSupplier(selectedSupplier);
            gvProducts2.ItemsSource = Prod;
            if (selectedSupplier != null)
            {
                Prod = ProductDB.GetSupProdNotInList(selectedSupplier);
                gvProdBySup.ItemsSource = Prod;
            }
        }

        //Sheila Zhao, with edits by Ethan Shipley February 1 2019
        // Clears the fields for the products page
        private void BtnClearS_Click(object sender, RoutedEventArgs e)
        {
            txtSupName.Text = "";
            btnDelPS.Visibility = Visibility.Hidden;
            btnAddPS.Visibility = Visibility.Hidden;
            gvProdBySup.Visibility = Visibility.Hidden;
            btnSaveS.Visibility = Visibility.Hidden;
            lblPNL.Visibility = Visibility.Hidden;
            btnShowSP.Content = "Show";
            UpdateBindingProdSup();
        }

        //Sheila Zhao, with edits by Ethan Shipley February 1 2019
        // Deletes the product for selected supplier
        private void BtnDelSP_Click(object sender, RoutedEventArgs e)
        {
            int SupID, ProdID;
            int Success = 0;

            if (gvProducts2.SelectedCells.Count > 0 && gvSuppliers2.SelectedCells.Count > 0)
            {
                // get the productID and the supplier ID
                ProdID = Convert.ToInt32(gvProducts2.SelectedItem.GetType().GetProperty("ProductId").GetValue(gvProducts2.SelectedItem, null));
                SupID = Convert.ToInt32(gvSuppliers2.SelectedItem.GetType().GetProperty("SupplierId").GetValue(gvSuppliers2.SelectedItem, null));

                try
                {
                    // Make new object
                    ProdSuppliers del = new ProdSuppliers();

                    del.ProdId = ProdID;
                    del.SupplierId = SupID;

                    // delete the product
                    Success = ProdSupplierDB.DeleteSupProduct(del);

                    if (gvProdSup_all_pkgs.Visibility == Visibility.Visible)
                    {
                        psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
                        gvProdSup_all_pkgs.ItemsSource = psn;
                        ProdSupListDetails(gvProdSup_all_pkgs);
                    }
                    

                    if (Success == 1)
                    {                        
                        try
                        {
                            UpdatePNL();
                            ScrollDown(gvProdBySup);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, ex.GetType().ToString());
                        }

                    }
                }
                catch (SqlException)
                {
                    MessageBox.Show("Product deleted Failed, Go Home");
                }
            }
        }

        //Sheila Zhao, with edits by Ethan Shipley February 1 2019
        // Adds the product for selected supplier
        private void BtnAddSP_Click(object sender, RoutedEventArgs e)
        {
            int ProdSupID, SupID;
            int Success = 0;

            if ((gvProdBySup.SelectedCells.Count > 0 && gvSuppliers2.SelectedCells.Count > 0) || (gvProducts2.SelectedCells.Count == 0))
            {
                // get supplierID and the productID
                SupID = Convert.ToInt32(gvSuppliers2.SelectedItem.GetType().GetProperty("SupplierId").GetValue(gvSuppliers2.SelectedItem, null));
                ProdSupID = Convert.ToInt32(gvProdBySup.SelectedItem.GetType().GetProperty("ProductId").GetValue(gvProdBySup.SelectedItem, null));

                try
                {
                    // Make new object
                    ProdSuppliers addNew = new ProdSuppliers();

                    addNew.ProdId = ProdSupID;
                    addNew.SupplierId = SupID;
                  
                    // add selected product to the list
                    Success = ProdSupplierDB.InsertSupProduct(addNew);

                    if (gvProdSup_all_pkgs.Visibility == Visibility.Visible)
                    {
                        psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
                        gvProdSup_all_pkgs.ItemsSource = psn;
                        ProdSupListDetails(gvProdSup_all_pkgs);
                    }

                    if (Success == 1)
                    {

                        // refresh gridview
                        try
                        {
                            UpdatePNL();
                            ScrollDown(gvProducts2);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, ex.GetType().ToString());
                        }

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
        }

        //Ethan Shipley February 1, 2019
        // Gets the selected datagrid row properties for product or supplier depending on user input. By default gets products
        private object GetSelected(DataGrid dg, string supprod="")
        {
            object row = dg.SelectedItem;
            if (row == null)
            {
                dg.SelectedIndex = (dg.Items.Count) - 1;
                row = dg.SelectedItem;
            }
            object p;
            if (supprod=="Supplier")
            {
                p = new Supplier(Convert.ToInt32(row.GetType().GetProperty("SupplierId").GetValue(row, null)),
                                    Convert.ToString(row.GetType().GetProperty("SupName").GetValue(row, null)));
            }
            else
            {
                p = new Products(Convert.ToInt32(row.GetType().GetProperty("ProductId").GetValue(row, null)),
                                    Convert.ToString(row.GetType().GetProperty("ProdName").GetValue(row, null)));
            }
            return p;
        }

        /*=======================================================================================================================*/
        /*=======================================================================================================================*/
        /*===================================================PRODUCT=============================================================*/
        /*=======================================================================================================================*/
        /*=======================================================================================================================*/

        //Sheila Zhao, with edits by Ethan Shipley
        //Displays all of the propucts available in the products gridview
        private void ProductList()
        {
            gvProducts1.ItemsSource = Prodd;
        }

        // Sheila Zhao
        // Gets the products itemsource for the products datagrid
        private void GetProduct(int productID)
        {
            Prod = ProductDB.GetProducts();
            selectProducts = (from Pd in Prod
                              where Pd.ProductId == productID
                              select Pd).ToList();

            gvProducts1.ItemsSource = selectProducts;

        }
        //SHeila Zhao, with edits by Ethan Shipley
        //Updates the item sources
        private void UpdateBindingProdSup()
        {
            gvProducts1.ItemsSource = Prodd;
            gvSuppliers2.ItemsSource = Supp;
        }

        //Sheila Zhao, Edits by ethan shipley
        // As the user makes selections of the products datagrid this method changes the supplier that are shown in its corresponding datagrid
        private void GvProducts1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Products selectedProduct = null;

            try
            {
                selectedProduct = (Products)GetSelected(gvProducts1);
                Sup = ProductDB.GetProductSuppliersByProduct(selectedProduct);
                gvSuppliers1.ItemsSource = Sup;
                if (selectedProduct != null)
                {
                    // also get the suppliers not on list with selected product
                    Sup = SupplierDB.GetProSupNotInList(selectedProduct);
                    gvSupByProd.ItemsSource = Sup;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }
        // Sheila Zhao
        // On button click saves a new product to the database
        private void BtnAddNewProd_Click(object sender, RoutedEventArgs e)
        {
            Products np = new Products();
            btnSaveP.Visibility = Visibility.Hidden;

            try
            {
                if (np != null)
                {
                    if (txtProdName.Text == "")
                    {
                        MessageBox.Show("Name can not be empty!");
                    }
                    else
                    {
                        // add the new product and save it
                        np.ProdName = txtProdName.Text;
                        ProductDB.InsertProduct(np);
                        Prodd = ProductDB.GetProducts();
                        UpdateBindingProdSup();
                        UpdateSNL();
                        UpdatePNL();

                        ScrollDown(gvProducts1);
                        //psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
                        //gvProdSup_all_pkgs.ItemsSource = psn;
                        //ProdSupListDetails(gvProdSup_all_pkgs);
                        //hideunhide(true);
                        ////ProdSupListDetails(gvProdSup_all_pkgs);
                        //gvProdSup_all_pkgs.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }

            txtProdName.Text = "";
        }

        // Sheila Zhao, with edits by Ethan Shipley
        // Allows editing of the products page
        private void BtnEditP_Click(object sender, RoutedEventArgs e)
        {
            Products oldp = null;
            btnSaveP.Visibility = Visibility.Visible;
            oldp = (Products)GetSelected(gvProducts1);
            txtProdName.Text = oldp.ProdName;
            txtProdName.Focus();
        }

        // Sheila Zhao, Edits by Ethan Shipley
        // Gets the edited products for the save function
        private Products GetEditProduct(Products oldp)
        {
            Products newp = new Products();
            oldp = (Products)GetSelected(gvProducts1);
            newp.ProdName = oldp.ProdName;
            newp.ProductId = oldp.ProductId;
            return newp;
        }

        // Sheila Zhao, edits by Ethan SHipley, January 30 2019
        // Upon user click of the save button, the edited product is changed in the database
        private void BtnSaveP_Click(object sender, RoutedEventArgs e)
        {
            Products newp = new Products();
            Products oldp = null;
            oldp = (Products)GetSelected(gvProducts1);
            if (txtProdName.Text == "")
            {
                MessageBox.Show("Name can not be empty!");
                return;
            }
            else
            {
                newp = GetEditProduct(oldp);
                try
                {
                    // save the new name to the database
                    newp.ProdName = txtProdName.Text;
                    ProductDB.UpdateProduct(newp, oldp);
                    Prodd = ProductDB.GetProducts();
                    UpdateBindingProdSup();
                    UpdatePNL();
                    UpdateSNL();
                    MessageBox.Show("Product Saved!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
                btnSaveP.Visibility = Visibility.Hidden;
                txtProdName.Text = "";
            }

        }
        //Sheila Zhao, with edits by Ethan Shipley
        // Clears the fields for the products page
        private void BtnClearP_Click(object sender, RoutedEventArgs e)
        {
            txtProdName.Text = "";
            btnDelSP.Visibility = Visibility.Hidden;
            btnAddSP.Visibility = Visibility.Hidden;
            btnSaveP.Visibility = Visibility.Hidden;
            gvSupByProd.Visibility = Visibility.Hidden;
            lblSNL.Visibility = Visibility.Hidden;
            btnShowPS.Content = "Show";
            UpdateBindingProdSup();
        }

        // Sheila Zhao, with edits by Ethan Shipley
        // Deletes the selected product from database when user clicks button
        private void BtnDelProd_Click(object sender, RoutedEventArgs e)
        {
            btnSaveP.Visibility = Visibility.Hidden;
            try
            {
                Products delProd = null;
                delProd = (Products)GetSelected(gvProducts1);
                if (!deleteConfirm())
                {
                    return;
                }
                ProductDB.DeleteProduct(delProd);
                txtProdName.Text = "";
                Prodd = ProductDB.GetProducts();
                UpdateBindingProdSup();
                UpdatePNL();
                UpdateSNL();
                ScrollDown(gvProducts1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        // Sheila Zhao, with edits by Ethan Shipley
        // SHows and hided the suppliers that are not connected to the product in products suppliers textbox
        private void BtnShowPS_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(btnShowPS.Content) == "Show")
            {
                gvSupByProd.Visibility = Visibility.Visible;
                btnDelPS.Visibility = Visibility.Visible;
                btnAddPS.Visibility = Visibility.Visible;
                lblSNL.Visibility = Visibility.Visible;

                btnShowPS.Content = "Hide";
                UpdateSNL();
                UpdatePNL();
                
            }
            else if (Convert.ToString(btnShowPS.Content) == "Hide")
            {
                gvSupByProd.Visibility = Visibility.Hidden;
                btnDelPS.Visibility = Visibility.Hidden;
                btnAddPS.Visibility = Visibility.Hidden;
                lblSNL.Visibility = Visibility.Hidden;
                btnShowPS.Content = "Show";

                UpdatePNL();
                UpdateSNL();
            }
        }


        private void UpdateSNL()
        {
            Products selectedProduct = null;
            selectedProduct = (Products)GetSelected(gvProducts1);

            // update both not on list and on list gridview
            Sup = ProductDB.GetProductSuppliersByProduct(selectedProduct);
            gvSuppliers1.ItemsSource = Sup;
            if (selectedProduct != null)
            {
                Sup = SupplierDB.GetProSupNotInList(selectedProduct);
                gvSupByProd.ItemsSource = Sup;
            }
        }
        // Sheila Zhao, with edits by Ethan Shipley
        // Deletes the supplier for the selected product
        private void BtnDelPS_Click(object sender, RoutedEventArgs e)
        {
            int ProdSupID, ProdID;
            int Success = 0;
            
            if (gvProducts1.SelectedCells.Count > 0 && gvSuppliers1.SelectedCells.Count > 0)
            {
                // get the productID and supplierID
                ProdID = Convert.ToInt32(gvProducts1.SelectedItem.GetType().GetProperty("ProductId").GetValue(gvProducts1.SelectedItem,null));
                ProdSupID = Convert.ToInt32(gvSuppliers1.SelectedItem.GetType().GetProperty("SupplierId").GetValue(gvSuppliers1.SelectedItem, null));

                try
                {

                    ProdSuppliers del = new ProdSuppliers();

                    del.ProdId = ProdID;
                    del.SupplierId = ProdSupID;

                    // delete the supplier
                    Success = ProdSupplierDB.DeleteProdSupplier(del);

                    if (gvProdSup_all_pkgs.Visibility == Visibility.Visible)
                    {
                        psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
                        gvProdSup_all_pkgs.ItemsSource = psn;
                        ProdSupListDetails(gvProdSup_all_pkgs);
                    }

                    if (Success == 1)
                    {

                        // Refresh gridview
                        try
                        {
                            UpdateSNL();

                            ScrollDown(gvSupByProd);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, ex.GetType().ToString());
                        }

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to delete");
            }
        }

        //Sheila Zhoa
        private void BtnAddPS_Click(object sender, RoutedEventArgs e)
        {
            int SupID, ProdID;
            int Success = 0;
            if ((gvProducts1.SelectedCells.Count > 0 && gvSupByProd.SelectedCells.Count > 0) || (gvSuppliers1.SelectedCells.Count == 0 && gvSupByProd.SelectedCells.Count > 0))
            {
                // get the productID and supplierID
                ProdID = Convert.ToInt32(gvProducts1.SelectedItem.GetType().GetProperty("ProductId").GetValue(gvProducts1.SelectedItem, null));
                SupID = Convert.ToInt32(gvSupByProd.SelectedItem.GetType().GetProperty("SupplierId").GetValue(gvSupByProd.SelectedItem, null));

                try
                {
                    // Make new object
                    ProdSuppliers addNew = new ProdSuppliers();

                    addNew.ProdId = ProdID;
                    addNew.SupplierId = SupID;
                    
                    // Insert the supplier to the selected product
                    Success = ProdSupplierDB.InsertProdSupplier(addNew);

                    if (gvProdSup_all_pkgs.Visibility == Visibility.Visible)
                    {
                        psn = ProdSuppliersNamesDB.GetProdSupAll(ppss);
                        gvProdSup_all_pkgs.ItemsSource = psn;
                        ProdSupListDetails(gvProdSup_all_pkgs);
                    }

                    if (Success == 1)
                    {

                        // Refresh gridview
                        try
                        {
                            UpdateSNL();
                            ScrollDown(gvSuppliers1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, ex.GetType().ToString());
                        }

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to add");
            }
        }
        //Ethan Shipley January 30 2019
        // Scroll to bottom
        private void ScrollDown(DataGrid dg)
        {
            if (dg.Items.Count > 0)
            {
                dg.UnselectAll();
                int nRowIndex = dg.Items.Count - 1;
                dg.SelectedIndex = nRowIndex;
                dg.Focus();
                dg.ScrollIntoView(dg.Items[dg.Items.Count - 1]);
                dg.UpdateLayout();
                dg.ScrollIntoView(dg.Items[nRowIndex]);
            }
        }
    }
}
