const host = "https://provinces.open-api.vn/api/";

const callAPI = (api, callback) => {
    return axios.get(api)
        .then((response) => {
            callback(response.data);
        })
        .catch((error) => {
            console.error("Có lỗi xảy ra:", error);
        });
};

callAPI(host + '?depth=1', (data) => renderData(data, "city"));

const renderData = (array, selectId) => {
    let options = '<option value="" selected>Chọn</option>';
    array.forEach(item => {
        options += `<option data-id="${item.code}" value="${item.name}">${item.name}</option>`;
    });
    document.getElementById(selectId).innerHTML = options;
};

$('#city').change(() => {
    const cityId = $("#city").find(':selected').data('id');
    callAPI(`${host}p/${cityId}?depth=2`, (data) => renderData(data.districts, "district"));
    $('#ward').html('<option value="">Chọn Phường/Xã</option>');
});

$('#district').change(() => {
    const districtId = $("#district").find(':selected').data('id');
    callAPI(`${host}d/${districtId}?depth=2`, (data) => renderData(data.wards, "ward"));
});

$('#ward').change(() => {
    printResult();
});

const printResult = () => {
    const city = $("#city option:selected").text();
    const district = $("#district option:selected").text();
    const ward = $("#ward option:selected").text();
    $("#result").text(`Địa chỉ: ${city} - ${district} - ${ward}`);
};
