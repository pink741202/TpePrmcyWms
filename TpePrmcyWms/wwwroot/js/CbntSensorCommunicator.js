
class CbntSensorCommunicator {
    constructor(cmuurl) {
        this.PostUrl = cmuurl;
    };

    //�[�J��C
    AddQu(qDrugCode, qtype) {
        this.TaskFlag_Sensors = 0;
        let returnresult = false;
        let newQu = { "DrugCode": qDrugCode, "OperType": qtype }
        if (!this.QuHadDrug.includes(qDrugCode)) { 
            let goPost = new AjaxJsonPostor("/FrontApi/AddQu", newQu);
            goPost.SuccessCallback = function (result) {
                newQu.returnresult = result.code == 0
                if (Cmutr.AddQuReact) { Cmutr.AddQuReact(newQu); }
            }; //console.log("in AddQu returnresult=" + returnresult)
            goPost.CompleteCallback = function () { Cmutr.TaskFlag_Sensors = 1; };
            goPost.Post();
        }
        else {
            newQu.returnresult = true;
            if (Cmutr.AddQuReact) { Cmutr.AddQuReact(newQu); }
            Cmutr.TaskFlag_Sensors = 1;
        }
        
    };
    AddQuReact = null;

    //�窱�A
    SetState(upQu) {
        this.TaskFlag_Sensors = 0;
        let returnresult = 0;
        let goPost = new AjaxJsonPostor("/FrontApi/SetState", upQu);
        goPost.SuccessCallback = function (result) {
            returnresult = result.code == 0;
            if (Cmutr.SetStateReact) { Cmutr.SetStateReact(returnresult); }
        };
        goPost.CompleteCallback = function () { this.TaskFlag_Sensors = 1; };
        goPost.Post();
        
    };
    SetStateReact = null;

    QuHadDrug = [];

    //#region �P�����j��
    TaskFlag_Sensors = 1;
    UpdateCallBack = null;
    DeleteCallBack = null;
    TikCallBack = null;
    //returnData = null;

    Running() {
        setInterval(this.ExeCommunicate.bind(this), 500);
    }
    ExeCommunicate() {
        //console.log("executing flag=" + this.TaskFlag_Sensors )
        let goPost = new AjaxJsonPostor("/FrontApi/GetQu", this.CmuTaskQu);
        goPost.SuccessCallback = function (result) {
            //console.log("Return=" + JSON.stringify(result.returnData))
            if (Cmutr.QuHadDrug.length > 0) { console.log("QuHadDrug=" + JSON.stringify(Cmutr.QuHadDrug)); }
            let res = result.returnData;
            if (res) {
                res.forEach((e) => {
                    //console.log("had=" + e.drugCode);
                    if (!Cmutr.QuHadDrug.includes(e.drugCode)) { Cmutr.QuHadDrug.push(e.drugCode); }
                    if (Cmutr.UpdateCallBack) { Cmutr.UpdateCallBack(e); }
                })
            }
            Cmutr.QuHadDrug.forEach((e) => {
                let r = res.find(x => x.drugCode == e);
                if (!r) { Cmutr.QuHadDrug.splice(Cmutr.QuHadDrug.indexOf(e), 1); }
                if (!r && Cmutr.DeleteCallBack) { Cmutr.DeleteCallBack(e); }
            });
            //��s�˼Ƶn�X
            if (Cmutr.QuHadDrug.length > 0) { SensorRunning(); }
        };
        goPost.CompleteCallback = function () {
            //console.log("complete");
            Cmutr.TaskFlag_Sensors++;
        };
        goPost.Post();

        if (Cmutr.TikCallBack) { Cmutr.TikCallBack(); }
    };

    //#endregion
}

//#region �l�����I�s�\�� �T�w
function SelectingDrawer(qDrugCode, sDrawFid) {
    let upQu = { "DrugCode": qDrugCode, "oprState": "0", "DrawFid": sDrawFid }
    Cmutr.SetState(upQu);
}
function SetState(qDrugCode, state) {
    let upQu = { "DrugCode": qDrugCode, "oprState": state }
    Cmutr.SetState(upQu);
}
function DeleteTask(qDrugCode) {
    let upQu = { "DrugCode": qDrugCode, "oprState": "D" }
    Cmutr.SetState(upQu);
}
function HideTask(qDrugCode) {
    let upQu = { "DrugCode": qDrugCode, "oprState": "H" }
    Cmutr.SetState(upQu);
}
    

//#endregion