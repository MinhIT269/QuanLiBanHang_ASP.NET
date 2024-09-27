$(document).ready(function () {
	// Hàm cập nhật giỏ hàng trong dropdown
	function updateCartDropdown() {
		$.ajax({
			url: '/cart.html/GetCartItems', //API để lấy danh sách sản phẩm trong giỏ hàng
			type: 'GET',
			success: function (cartItems) {
				let cartHtml = '';
				let totalPrice = 0;
				cartItems.forEach(item => {
					cartHtml += `
								<div class="product">
									<div class="product-details">
										<h4 class="product-title">
											<a href="/product/productdetail?id=${item.productId}">${item.name}</a>
										</h4>
										<span class="cart-product-info">
											<span class="cart-product-qty">${item.quantity}</span> × ${item.price.toLocaleString('vi-VN')}₫
										</span>
									</div>
									<figure class="product-image-container">
										 <a href="/product/productdetail?id=${item.productId}" class="product-image">
											<img src="${item.image}" alt="product" width="80" height="80">
										</a>
										<a href="#" class="btn-remove" data-product-id="${item.productId}" title="Remove Product"><span>×</span></a>
									</figure>
								</div>
							`;
					totalPrice += item.price * item.quantity;
				});
				// Cập nhật nội dung giỏ hàng
				$('.dropdown-cart-products').html(cartHtml);
				$('.cart-total-price').text(`${totalPrice.toLocaleString('vi-VN')}₫`);
			},
			error: function (xhr) {
				console.log('Error loading cart items: ' + xhr.responseText);
			}
		});
	}


	// Hàm gọi API để cập nhật số lượng sản phẩm trong giỏ hàng
	function updateCartCount() {
		$.ajax({
			url: '/cart.html/CartItemCount',  // Gọi API vừa tạo
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

	updateCartDropdown();  // Gọi hàm cập nhật giỏ hàng khi trang được tải

	// Hàm lắng nghe sự kiện click để cập nhật giỏ hàng
	$('.btn-update-cart').click(function (event) {
		event.preventDefault(); // Ngăn chặn hành động mặc định

		// Tạo mảng chứa sản phẩm và số lượng cập nhật
		let updatedCartItems = [];
		$('#cart-items .product-row').each(function () {
			let productId = $(this).find('.btn-remove').data('product-id');
			let quantity = parseInt($(this).find('.horizontal-quantity').val());

			updatedCartItems.push({
				ProductId: productId,
				Quantity: quantity
			});
		});

		// Gửi AJAX để cập nhật giỏ hàng
		$.ajax({
			url: '/cart.html/UpdateCart', // API để cập nhật giỏ hàng
			type: 'PUT',
			contentType: 'application/json',
			data: JSON.stringify(updatedCartItems),
			success: function (response) {
				alert('Cart updated successfully!');
				updateCartDropdown(); // Cập nhật lại dropdown giỏ hàng
				loadCartItems(); // Tải lại sản phẩm trong giỏ hàng
			},
			error: function (xhr) {
				alert('Error: ' + xhr.responseText);
			}
		});
	});

	// Xử lý khi người dùng thêm sản phẩm vào giỏ hàng
	$('.btn-add-cart').click(function (event) {
		event.preventDefault(); // Ngăn chặn hành động mặc định của thẻ <a>

		const productId = $(this).data('product-id');
		const quantity = $(this).data('quantity');
		const name = $(this).data('animation-name');
		const price = parseFloat($(this).data('price'));
		const image = $(this).data('image');
		// Kiểm tra giá trị price trong console để đảm bảo nó là số
		/*console.log("Price:", image);
		console.log("Price:", name);*/
		$.ajax({
			url: '/cart.html/AddtoCart',
			type: 'POST',
			contentType: 'application/json',
			data: JSON.stringify({
				ProductId: productId,
				quantity: quantity,
				Name: name,
				Price: price,
				Image: image
			}),
			success: function (response) {
				alert(response); // Hiển thị thông báo thành công
				$('.view-cart').removeClass('d-none'); // Xóa lớp d-none để hiển thị nút
				updateCartCount(); // Gọi lại hàm để cập nhật số lượng sản phẩm trên giỏ hàng sau khi thêm
				updateCartDropdown(); // Cập nhật dropdown giỏ hàng
			},
			error: function (xhr) {
				alert('Error: ' + xhr.responseText); // Hiển thị thông báo lỗi
			}
		});
	});
	function loadCartItems() {
		$.ajax({
			url: '/cart.html/GetCartItems', //API để lấy danh sách sản phẩm trong giỏ hàng
			type: 'GET',
			success: function (cartItems) {
				let cartHtml = '';
				let totalPrice = 0;
				cartItems.forEach(item => {
					cartHtml += `
							<tr class="product-row">
								<td>
									<figure class="product-image-container">
										<a href="product.html" class="product-image">
											<img src="${item.image}" alt="product">
										</a>
										<a href="#" class="btn-remove icon-cancel" title="Remove Product" data-product-id="${item.productId}"></a>
									</figure>
								</td>
								<td class="product-col">
									<h5 class="product-title">
										<a href="product.html">${item.name}</a>
									</h5>
									<span style="font-size: 0.8em;">Còn hàng: <strong>${item.stock}</strong></span>
								</td>
								<td>${item.price.toLocaleString('vi-VN')}₫</td>
								<td>
									<div class="product-single-qty">
											  <div class="input-group bootstrap-touchspin bootstrap-touchspin-injected">
													<span class= "input-group-btn input-group-prepend">
															   <button class = "btn btn-outline btn-down-icon bootstrap-touchspin-down qty-decrement" type = "button"> </button>
											        </span>
												      <input class="horizontal-quantity form-control" type="text" value="${item.quantity}">
													<span class= "input-group-btn input-group-prepend">
															   <button class = "btn btn-outline btn-up-icon bootstrap-touchspin-up qty-increment" type = "button"> </button>
												    </span>
									  </div>
									</div>
								</td>
								<td class="text-right">
										<span class="subtotal-price" data-price="${item.price}">${(item.price * item.quantity).toLocaleString('vi-VN')}₫</span>
								</td>
							</tr>
						`;
					totalPrice += item.price * item.quantity;
				});

				$('#cart-items').html(cartHtml);
			},
			error: function (xhr) {
				console.log('Error loading cart items: ' + xhr.responseText);
			}
		});
	}

	loadCartItems();

	$(document).on('click', '.qty-decrement', function () {
		let qtyInput = $(this).closest('.input-group').find('.horizontal-quantity');
		let currentValue = parseInt(qtyInput.val());
		if (currentValue > 1) {
			qtyInput.val(currentValue - 1);
			updateSubtotal(qtyInput);
		}
	});

	$(document).on('click', '.qty-increment', function () {
		let qtyInput = $(this).closest('.input-group').find('.horizontal-quantity');
		let currentValue = parseInt(qtyInput.val());
		qtyInput.val(currentValue + 1);
		updateSubtotal(qtyInput);
	});
	function updateSubtotal(qtyInput) {
		// Cập nhật lại subtotal
		let row = qtyInput.closest('.product-row');
		let price = parseFloat(row.find('.subtotal-price').attr('data-price')); // Dùng attr để lưu giá gốc
		let quantity = parseInt(qtyInput.val());
		let subtotal = price * quantity;

		// Cập nhật lại subtotal hiển thị
		row.find('.subtotal-price').text(subtotal.toLocaleString('vi-VN') + '₫');
	}
	// Xử lý sự kiện khi nhấn vào nút xóa sản phẩm
	$(document).on('click', '.btn-remove', function (event) {
		event.preventDefault();
		const productId = $(this).data('product-id');

		$.ajax({
			url: `/cart.html/Remove/${productId}`,
			type: 'DELETE',
			success: function (response) {
				alert(response);
				updateCartCount();
				updateCartDropdown(); // Cập nhật dropdown giỏ hàng sau khi xóa
				// Tải lại giỏ hàng
				loadCartItems();
			},
			error: function (xhr) {
				alert('Error: ' + xhr.responseText);
			}
		});
	});
});