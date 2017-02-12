function init(){

}

function UnityProgress (dom)
{
	this.progress = 0.0;
	this.message = "";
	this.dom = dom;

	
	createjs.CSSPlugin.install(createjs.Tween);
	createjs.Ticker.setFPS(60);

	this.SetProgress = function (progress)
	{
		if (this.progress < progress)
			this.progress = progress;

		if(progress < 0.5){
			this.SetMessage("Aguarde...");
		}
		else if(progress >= 0.5 && progress < 1){
			this.SetMessage("Trabalhando...");
		}
		else if (progress == 1){
			this.SetMessage("Iniciando o Sonho..");
			document.getElementById("spinner").style.display = "block";
			document.getElementById("bgBar").style.display = "none";
			document.getElementById("progressBar").style.display = "none";
			document.getElementById("loadingInfo").style.display = "none";
		}
		else if (progress >= 1) {
			loadingBox.style.display = "none";
		}

		this.Update();
	}

	this.SetMessage = function (message)
	{
		this.message = message;
		this.Update();
	}

	this.Clear = function()
	{
		document.getElementById("loadingBox").style.display = "none";
	}

	this.Update = function()
	{
		var lenght = 200 * Math.min(this.progress, 1);
		bar = document.getElementById("progressBar");
		createjs.Tween.removeTweens(bar);
		createjs.Tween.get(bar).to({width:lenght}, 500, createjs.Ease.sineOut);
		bar.style.width = lenght + "px";
		document.getElementById("loadingInfo").innerHTML = this.message;
	}

	this.Update ();
}