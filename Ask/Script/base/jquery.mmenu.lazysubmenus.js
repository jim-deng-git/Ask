/*	
 * jQuery mmenu lazySubmenus add-on
 * mmenu.frebsite.nl
 *
 * Copyright (c) Fred Heusschen
 */
!function(n){var e="mmenu",s="lazySubmenus";n[e].addons[s]={setup:function(){var t=this.opts[s];this.conf[s];a=n[e].glbl,"boolean"==typeof t&&(t={load:t}),"object"!=typeof t&&(t={}),t=this.opts[s]=n.extend(!0,{},n[e].defaults[s],t),t.load&&(this.bind("initMenu:after",function(){this.$pnls.find("li").children(this.conf.panelNodetype).not("."+i.inset).not("."+i.nolistview).not("."+i.nopanel).addClass(i.lazysubmenu+" "+i.nolistview+" "+i.nopanel)}),this.bind("initPanels:before",function(n){n=n||this.$pnls.children(this.conf.panelNodetype),this.__findAddBack(n,"."+i.lazysubmenu).not("."+i.lazysubmenu+" ."+i.lazysubmenu).removeClass(i.lazysubmenu+" "+i.nolistview+" "+i.nopanel)}),this.bind("initOpened:before",function(){var n=this.$pnls.find("."+this.conf.classNames.selected).closest("."+i.lazysubmenu);n.length&&this.initPanels(n)}),this.bind("openPanel:before",function(n){$panels=this.__findAddBack(n,"."+i.lazysubmenu).not("."+i.lazysubmenu+" ."+i.lazysubmenu),$panels.length&&this.initPanels($panels)}))},add:function(){i=n[e]._c,t=n[e]._d,l=n[e]._e,i.add("lazysubmenu"),t.add("lazysubmenu")},clickAnchor:function(n,e){}},n[e].defaults[s]={load:!1},n[e].configuration[s]={};var i,t,l,a}(jQuery);