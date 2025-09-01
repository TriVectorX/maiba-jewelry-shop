using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Reminders;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-09-12 12:00:00", "Reminders migration", MigrationProcessType.Update)]
public class RemindersMigration : Migration
{
    #region Fields

    private readonly IRepository<EmailAccount> _emailAccountRepository;
    private readonly IRepository<MessageTemplate> _messageTemplateRepository;
    private readonly IRepository<ScheduleTask> _scheduleTaskRepository;

    #endregion

    #region Ctor

    public RemindersMigration(IRepository<EmailAccount> emailAccountRepository, IRepository<MessageTemplate> messageTemplateRepository, IRepository<ScheduleTask> scheduleTaskRepository)
    {
        _emailAccountRepository = emailAccountRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _scheduleTaskRepository = scheduleTaskRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {

        var eaGeneral = _emailAccountRepository.Table.FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");

        #region Abandoned cart

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE,
                Subject = "%Customer.FirstName%, you left some items in your cart. %Store.Name%.",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>we noticed you left an item in your cart.</p>{Environment.NewLine}<p>Your shopping cart currently contains the following items:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 2,
                DelayPeriod = MessageDelayPeriod.Hours,
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE,
                Subject = "%Customer.FirstName%, you left some items in your cart. %Store.Name%.",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>we noticed you left an item in your cart.</p>{Environment.NewLine}<p>Your shopping cart currently contains the following items:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 1,
                DelayPeriod = MessageDelayPeriod.Days,
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE,
                Subject = "%Customer.FirstName%, you left some items in your cart. %Store.Name%.",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>we noticed you left an item in your cart.</p>{Environment.NewLine}<p>Your shopping cart currently contains the following items:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 5,
                DelayPeriod = MessageDelayPeriod.Days,
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, RemindersDefaults.AbandonedCarts.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process abandoned carts",
                    Seconds = 20 * 60,
                    Type = RemindersDefaults.AbandonedCarts.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion

        #region Pending order

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE,
                Subject = "You haven’t completed the order",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}We noticed that you haven’t completed the payment for your order on <a href=\"%Store.URL%\">%Store.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 3,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE,
                Subject = "The payment has not been completed",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}We noticed that you haven’t completed the payment for your order on <a href=\"%Store.URL%\">%Store.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 10,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, RemindersDefaults.PendingOrders.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process incomplete orders",
                    Seconds = 60 * 60,
                    Type = RemindersDefaults.PendingOrders.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion

        #region Incomplete registration

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE,
                Subject = "Complete registration at %Store.Name%.",
                Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 1,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, RemindersDefaults.IncompleteRegistrations.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process incomplete registrations",
                    Seconds = 60 * 60,
                    Type = RemindersDefaults.IncompleteRegistrations.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion

    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }

    #endregion
}
