/*
//version1
File Name   : main-new.js
Author      : Bang Daejong
*/

window.onload = function() {
    controlGNB();
}

function controlGNB(){
    var showroomGNB = document.getElementById('gnb-showroom');
    var showroomSNB = document.getElementById('snb-showroom');
    var shoppingFinanceGNB = document.getElementById('gnb-shoppingFinance');
    var shoppingFinanceSNB = document.getElementById('gnb-shoppingFinance').getElementsByTagName('div')[0];
    var serviceOwnersGNB = document.getElementById('gnb-serviceOwners');
    var serviceOwnersSNB = document.getElementById('gnb-serviceOwners').getElementsByTagName('div')[0];
		var etcGNB = document.getElementById('gnb-etc');
	
		//**** 20110329 add
		var etcSNB = null;
		if (etcGNB != null && etcGNB != "undefined"){
			if(etcGNB.getElementsByTagName('div').length > 0)
			etcSNB = etcGNB.getElementsByTagName('div')[0]
		}
		//**** 20110329 end
	
	//alert('11111111111111');	
    //var etcSNB = document.getElementById('gnb-etc').getElementsByTagName('div')[0];
    

    //var gnbs = [showroomGNB, shoppingFinanceGNB, serviceOwnersGNB, newsEventsGNB, etcGNB];
    //var snbs = [showroomSNB, shoppingFinanceSNB ,serviceOwnersSNB, newsEventsSNB, etcSNB];
	var gnbs = [showroomGNB, shoppingFinanceGNB, etcGNB];
    var snbs = [showroomSNB, shoppingFinanceSNB, etcSNB];
    var index = -1;
    var timer = null;

    function overGNB(idx) {
        clearTimeout(timer);

        if (index != idx) {
            index = idx;

            for (var i = 0; i < gnbs.length; i++) {
            	//*** 20110329 수정
    						if( gnbs[i] == null || gnbs[i] == "undefined") continue;
                	jQuery(snbs[i]).stop();
            	//*** 20110329 끝

                if (i == index) {
                    gnbs[i].getElementsByTagName('a')[0].className = 'gnb on';
                    snbs[i].style.height = 'auto';
                    snbs[i].style.minHeight = 0;

                    jQuery(snbs[i]).slideDown();
                }
                else {
                    gnbs[i].getElementsByTagName('a')[0].className = 'gnb';
                    snbs[i].style.display = 'none';
                }
            }
        }
    };

    var outGNB = function() {
        clearTimeout(timer);

        timer = setTimeout(function() {
            index = -1;

            for (var i = 0; i < gnbs.length; i++) {
            	//*** 20110329 수정
    					if( gnbs[i] == null || gnbs[i] == "undefined") continue;
            	//*** 20110329 끝
   					
    					
							gnbs[i].getElementsByTagName('a')[0].className = 'gnb';
              jQuery(snbs[i]).slideUp();
            }
        }, 500);
    };

    for (var i = 0; i < gnbs.length; i++) {
    	//*** 20110329 수정
    	if( gnbs[i] == null || gnbs[i] == "undefined") continue;
    	//*** 20110329 끝
    	
      gnbs[i].onmouseover = snbs[i].onmouseover = function() {
          var idx = i;
          return function(){ overGNB(idx); };
      }();

      gnbs[i].onmouseout = outGNB;
    }

    var articlePassengerLi = document.getElementById('article-passenger').getElementsByTagName('li');
    var articleRecreationalLi = document.getElementById('article-recreational').getElementsByTagName('li');

    for(var i=0;i<articlePassengerLi.length;i++){
        articlePassengerLi[i].onmouseover = function(){
            this.getElementsByTagName('p')[0].style.color = '#0f5699';

            this.onmouseout = function(){
                this.getElementsByTagName('p')[0].style.color = '#666';
            }
        }
        articlePassengerLi[i].onclick = function(){
            location.href = this.getElementsByTagName('a')[0].href;
        }
    }

    for(var i=0;i<articleRecreationalLi.length;i++){
        articleRecreationalLi[i].onmouseover = function(){
            this.getElementsByTagName('p')[0].style.color = '#0f5699';

            this.onmouseout = function(){
                this.getElementsByTagName('p')[0].style.color = '#666';
            }
        }
        articleRecreationalLi[i].onclick = function(){
            location.href = this.getElementsByTagName('a')[0].href;
        }
    }
}

