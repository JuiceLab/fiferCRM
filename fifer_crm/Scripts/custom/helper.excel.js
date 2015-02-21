function saveExcel(paymentId, type)
{
    location.href = "/Finances/Payment/GetPaymentExcel?paymentId=" + paymentId + "&type=" + type;
}