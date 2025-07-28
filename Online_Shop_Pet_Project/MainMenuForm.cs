
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project
{
    public partial class MainMenuForm : Form
    {
        public bool isEmployee;
        public string userRole;

        // Панели интерфейса
        public Panel productsPanel, productDetailsPanel, ordersPanel, cartPanel, deliveryPanel, profilePanel,
            deliveriesPanel, routePanel, earningsPanel, cookOrdersPanel, cookMenuPanel, ingredientsPanel,
            hallStaffOrdersPanel, storeMapPanel, hallStaffHistoryPanel, helpPanel, ticketsPanel, historyPanel,
            offlineOrderPanel, returnPanel, chatPanel, complaintsPanel, chatSupportPanel, knowledgePanel;

        public ChatTicket currentChat;
        public List<Product> products = new List<Product>();
        public List<Order> orders = new List<Order>();
        public Order currentOrder = new Order();
        public string deliveryMethod = "Самовывоз";
        public string paymentMethod = "Сразу";

        public UserProfile userProfile = new UserProfile
        {
            Name = "Иван Иванов",
            Phone = "+7 (123) 456-78-90",
            Email = "ivan.ivanov@example.com",
            Password = "********",
            PhotoPath = "E:\\с#\\Online_Shop_Pet_Project\\Online_Shop_Pet_Project\\art\\Person.png"
        };

        // Хелпер-классы
        public ProductHelper ProductHelper;
        public OrderHelper OrderHelper;
        public CartHelper CartHelper;
        public DeliveryHelper DeliveryHelper;
        public ProfileHelper ProfileHelper;
        public EmployeeHelper EmployeeHelper;
        public SupportHelper SupportHelper;
        public UIHelper UIHelper;
        public CookHelper CookHelper;
        public HallStaffHelper HallStaffHelper;
        public SellerHelper SellerHelper;

        public MainMenuForm(bool isEmployee)
        {
            InitializeComponent();
            this.isEmployee = isEmployee;
            this.WindowState = FormWindowState.Maximized;

            // Инициализация хелпер-классов
            ProductHelper = new ProductHelper(this);
            OrderHelper = new OrderHelper(this);
            CartHelper = new CartHelper(this);
            DeliveryHelper = new DeliveryHelper(this);
            ProfileHelper = new ProfileHelper(this);
            EmployeeHelper = new EmployeeHelper(this);
            SupportHelper = new SupportHelper(this);
            UIHelper = new UIHelper(this);
            CookHelper = new CookHelper(this);
            HallStaffHelper = new HallStaffHelper(this);
            SellerHelper = new SellerHelper(this);

            // Инициализация данных
            ProductHelper.InitializeProducts();
            OrderHelper.InitializeOrders();
            UIHelper.InitializeUI();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (isEmployee)
            {
                EmployeeHelper.ShowEmployeeRoleSelection();
            }
            else
            {
                UIHelper.ShowCustomerMenu();
                ProductHelper.ShowProductsPanel();
            }
        }
    }
}