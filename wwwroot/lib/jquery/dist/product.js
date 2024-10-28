$(document).ready(function () {
	let currentPage = 1; // Trang hiện tại
	const maxVisiblePages = 5; // Số lượng trang tối đa hiển thị
	let searchQuery = '';
	let sortCriteria = ''; // Biến lưu trữ từ khóa sắp xếp
	let idCat = ''; // Biến lưu trữ id category
	let itemsPerPage = 12; // Số sản phẩm mỗi trang

	// Lấy giá trị từ URL query string nếu có
	const urlParams = new URLSearchParams(window.location.search);
	searchQuery = urlParams.get('searchQuery') || '';
	idCat = urlParams.get('idCat') || '';

	// Lắng nghe sự kiện submit của form tìm kiếm
	document.querySelector('form').addEventListener('submit', function (event) {
		event.preventDefault(); // Ngăn chặn hành động mặc định của form (reload trang)
		searchQuery = document.getElementById('q').value; // Lấy từ khóa tìm kiếm từ input
		idCat = document.getElementById('cat').value; // Lấy id category từ dropdown
		currentPage = 1; // Reset về trang đầu tiên
		performSearch(); // Thực hiện tìm kiếm
	});

	// Lắng nghe sự kiện thay đổi của dropdown menu
	document.getElementById('itemsPerPageSelect').addEventListener('change', function (event) {
		itemsPerPage = parseInt(event.target.value); // Cập nhật giá trị của itemsPerPage
		currentPage = 1; // Reset về trang đầu tiên khi thay đổi số lượng sản phẩm mỗi trang
		performSearch(); // Thực hiện tìm kiếm lại với số lượng sản phẩm mới
	});

	// Lắng nghe sự kiện thay đổi của dropdown sắp xếp
	document.querySelector('select[name="orderby"]').addEventListener('change', function (event) {
		sortCriteria = event.target.value; // Cập nhật tiêu chí sắp xếp
		currentPage = 1; // Reset về trang đầu tiên khi thay đổi tiêu chí sắp xếp
		performSearch(); // Thực hiện tìm kiếm lại với tiêu chí sắp xếp mới
	});

	async function GetTotalPages() {
		try {
			const url = `/product/totalPages?searchQuery=${encodeURIComponent(searchQuery)}&idCat=${encodeURIComponent(idCat)}&itemsPerPage=${encodeURIComponent(itemsPerPage)}`;
			console.log('Request URLPage:', url);
			const response = await $.ajax({
				url: url,
				type: 'GET',
				dataType: 'json'
			});
			console.log('Total pages:', response.totalPages); 
			return response.totalPages; // Giả sử API trả về một đối tượng với thuộc tính totalPages
		} catch (error) {
			console.error('Error fetching total pages:', error);
			return 1; // Trả về 1 trang nếu có lỗi
		}
	}

	async function GetProduct(textsearch, page, limit, sortCriteria, idCat) {
		try {
			const url = `/product/search/list?searchQuery=${encodeURIComponent(textsearch)}&page=${page}&limit=${limit}&sort=${encodeURIComponent(sortCriteria)}&idCat=${encodeURIComponent(idCat)}`;
/*			console.log('Request URL:', url);*/
			const data = await $.ajax({
				url: url,
				type: 'GET',
				dataType: 'json'
			});
			/*console.log('Data received:', data);*/
			return data; // Trả về dữ liệu nhận được từ API
		} catch (error) {
			console.error('Error fetching products:', error);
			return []; // Trả về mảng rỗng nếu có lỗi
		}
	}

	// Quản lý việc hiển thị các số trang trên thanh phân trang
	async function renderPagination() {
		const totalPages = await GetTotalPages(); // Lấy tổng số trang từ API

		const pagination = document.getElementById('pagination');
		pagination.innerHTML = '';

		// Tính toán phạm vi các trang cần hiển thị
		let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
		let endPage = Math.min(totalPages, currentPage + Math.floor(maxVisiblePages / 2));

		// Điều chỉnh start và end page nếu số lượng trang không đủ để hiển thị
		if (endPage - startPage + 1 < maxVisiblePages) {
			if (endPage === totalPages) {
				startPage = Math.max(1, endPage - maxVisiblePages + 1);
			} else {
				endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
			}
		}

		// Nút Previous
		pagination.innerHTML += `
										<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
													<a class="page-link" href="javascript:void(0);" onclick="changePage(${currentPage - 1})"> <i class="icon-angle-left"></i></a>
										</li>
									`;

		// Các số trang
		for (let i = startPage; i <= endPage; i++) {
			pagination.innerHTML += `
											<li class="page-item ${i === currentPage ? 'active' : ''}">
												<a class="page-link" href="javascript:void(0);" onclick="changePage(${i})">${i}</a>
											</li>
										`;
		}

		// Nút Next
		pagination.innerHTML += `
										<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
													<a class="page-link" href="javascript:void(0);" onclick="changePage(${currentPage + 1})"> <i class="icon-angle-right"></i></a>
										</li>
									`;
	}

	// Hàm để chuyển trang
	window.changePage = async function (page) {
		if (page < 1 || page > await GetTotalPages()) return; // Kiểm tra trang hợp lệ
		currentPage = page;
		await renderTable(currentPage); // Hiển thị sản phẩm tương ứng với trang
		await renderPagination(); // Hiển thị phân trang
	};

	// Hàm để hiển thị sản phẩm tương ứng với trang
	async function renderTable(page) {
		const paginatedProducts = await GetProduct(searchQuery, page, itemsPerPage, sortCriteria, idCat);
		const tableBody = document.getElementById('productTableBody');
		tableBody.innerHTML = ''; // Xóa nội dung hiện tại của bảng

		if (paginatedProducts.length === 0) {
			tableBody.innerHTML = '<tr><td colspan="5">No products found</td></tr>';
			return;
		}

		paginatedProducts.forEach(item => {
			tableBody.innerHTML += HTMLProduct(item); // Chèn dữ liệu vào bảng
		});
	}

	// Hàm để tạo HTML cho một sản phẩm
	function HTMLProduct(item) {
		return `
						<div class="col-6 col-sm-4 col-md-3">
							<div class="product-default">
								<figure>
									<a href="/product/productdetail?id=${item.productId}">
										 <img src="${item.image}" width="300" height="300" alt="${item.name}" style="width: 300px; height: 300px;" />
									</a>
									<div class="label-group">
										<div class="product-label label-hot">HOT</div>
										<div class="product-label label-sale">-20%</div>
									</div>
								</figure>
								<div class="product-details">
									<div class="category-wrap">
										<div class="category-list">
											<a href="category.html" class="product-category">Category ${item.categoryName}</a>
										</div>
									</div>
									<h3 class="product-title">
										<a href="/product/productdetail?id=${item.productId}">${item.name}</a>
									</h3>
									<div class="ratings-container">
										<div class="product-ratings">
											<span class="ratings" style="width:100%"></span>
											<span class="tooltiptext tooltip-top"></span>
										</div>
									</div>
									<div class="price-box">
										<span class="old-price">${formatCurrency(item.price * 2)}</span>
										<span class="product-price">${formatCurrency(item.price)}</span>
									</div>
							        <div class="stock-info">
										<span style="font-size: 0.8em;">Còn hàng: <strong>${item.stock}</strong></span>
									</div>
									<div class="product-action">
										<a href="wishlist.html" class="btn-icon-wish" title="wishlist">
											<i class="icon-heart"></i>
										</a>
							     <form action="/product/productdetail" method="GET" style="display: inline;">
									<input type="hidden" name="id" value="${item.productId}" />
									<button type="submit" class="btn-icon btn-add-cart">
												<i class="fa fa-arrow-right"></i><span>SELECT OPTIONS</span>
									</button>
								 </form>
	
										<a href="ajax/product-quick-view.html" class="btn-quickview" title="Quick View">
											<i class="fas fa-external-link-alt"></i>
										</a>
									</div>
								</div>
							</div>
						</div>
					`;
	}

	// Hàm format currency
	function formatCurrency(value) {
		return value.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
	}

	// Hàm thực hiện tìm kiếm
	async function performSearch() {
		await renderTable(currentPage); // Hiển thị sản phẩm theo trang
		await renderPagination(); // Hiển thị phân trang
	}

	// Hàm cập nhật số lượng sản phẩm trong giỏ hàng
	function updateCartCount() {
		$.ajax({
			url: '/cart.html/CartItemCount', // Gọi API vừa tạo
			type: 'GET',
			success: function (itemCount) {
				$('.cart-count').text(itemCount); // Cập nhật số lượng trên giỏ hàng
			},
			error: function (xhr) {
				console.log('Error loading cart count: ' + xhr.responseText);
			}
		});
	}
	updateCartCount(); // Gọi hàm cập nhật số lượng khi trang được tải

	// Xử lý khi người dùng thêm sản phẩm vào giỏ hàng
	$('.btn-add-cart').click(function (event) {
		event.preventDefault(); // Ngăn chặn hành động mặc định của thẻ <a>

		const productId = $(this).data('product-id');
		const quantity = $(this).data('quantity');
		$.ajax({
			url: '/cart.html/AddtoCart',
			type: 'POST',
			contentType: 'application/json',
			data: JSON.stringify({
				ProductId: productId,
				quantity: quantity
			}),
			success: function (response) {
				alert(response); // Hiển thị thông báo thành công
				updateCartCount(); // Gọi lại hàm để cập nhật số lượng
			},
			error: function (xhr) {
				alert('Error: ' + xhr.responseText); // Hiển thị thông báo lỗi
			}
		});
	});

	// Khởi tạo
	performSearch();
});
