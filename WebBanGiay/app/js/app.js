const btnToggle = document.querySelector('#btnToggle');
const subMenu = document.querySelector('.sub-menu');
const arr = document.querySelectorAll('.giohang .list__content .item__details .inputBox');
const checkAll = document.getElementById('checkAll');
const openModal = document.getElementById('openModal');
const modal = document.getElementById('modal');
const search = document.getElementById('searchProducts');
const exitModal = document.querySelector('.modal__header a.exit');
let listCB = document.querySelectorAll('.admin .content table tbody input[type="checkbox"]');

const password = document.querySelector('#Password');
const repassword = document.querySelector('#Re-password');

const btnCheckoutShoppingCart = document.querySelector('#CheckoutShoppingCart');
const listItem = document.querySelectorAll('.item');

const Userlogout = document.querySelector('#Userlogout');

const fileupload = document.getElementById('fileupload');
const image = document.getElementById('image');

if(search != null){
    search.addEventListener('keyup',function(event){
        if(event.keyCode === 13){
            console.log(event.targe.value)
        }
    })
}

if(arr != null){
    arr.forEach(listBox => {
        listBox.childNodes[1].addEventListener('click',function(){
            if(parseInt(listBox.childNodes[3].value,10) > 1){
                listBox.childNodes[3].value = parseInt(listBox.childNodes[3].value,10) - 1;
             }
        })
        listBox.childNodes[5].addEventListener('click',function(){
              listBox.childNodes[3].value = parseInt(listBox.childNodes[3].value,10) + 1;
        })
    });
}


if(checkAll != null){
    checkAll.addEventListener('click',function(){
    
        if(checkAll.checked){
            listCB.forEach((item)=>{
                item.checked = true;
            })
        }else{
            listCB.forEach((item)=>{
                item.checked = false;
            })
        }
        let arr = [...listCB].map((item)=>{
            if(item.checked){
                return item.value;
            }
        })
    })
}

if(listCB != null){
    listCB.forEach(function(item){
        item.addEventListener('click',function(){
            if(!item.checked && checkAll.checked){
                checkAll.checked = false;
            }
        })
    
    })
}

if( openModal != null && openModal != null){
    openModal.addEventListener('click',function(){
        modal.classList.remove('isClose')
            modal.classList.add('isOpen')
    })
    exitModal.addEventListener('click',function(){
        modal.classList.remove('isOpen')
        modal.classList.add('isClose')
    })
}

if(btnToggle != null){
    btnToggle.addEventListener('click',function(){
        if(btnToggle.classList.contains('isOpen')){
            btnToggle.classList.remove('isOpen')
            subMenu.classList.remove('open')
        }else{
            btnToggle.classList.add('isOpen')
            subMenu.classList.add('open')
        }
    })
}


if (btnCheckoutShoppingCart != null) {
    btnCheckoutShoppingCart.addEventListener('click', function () {
        if (listItem.length==0) {
            window.alert("There are nothing in ShoppingCart");
        }
        else {
            window.alert("Success! Please check your email in few minute later !");
        }
    })
}
if (repassword != null)
{
    repassword.onkeyup = function () {
        if (password.value != repassword.value) {
            repassword.classList.remove("valid");
            repassword.classList.add("invalid");
            password.classList.remove("valid");
            password.classList.add("invalid");
        }
        else {
            repassword.classList.remove("invalid");
            repassword.classList.add("valid");
            password.classList.remove("invalid");
            password.classList.add("valid");
        }
    }
}

if (Userlogout != null)
{
    Userlogout.addEventListener('click', function () {
        setTimeout(function () {
            window.location.href = "https://localhost:44342/";
        }, 3000);
    })
}


