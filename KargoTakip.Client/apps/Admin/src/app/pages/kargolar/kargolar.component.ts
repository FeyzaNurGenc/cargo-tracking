import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, linkedSignal, resource, signal, ViewEncapsulation } from '@angular/core';
import { RouterLink } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { ODataModel } from '../../models/odata.model';
import { FlexiGridFilterDataModel, FlexiGridModule, FlexiGridService, StateModel } from 'flexi-grid';
import { BreadcrumbService } from '../../services/breadcrumb.service';
import BlankComponent from '../../components/blank/blank.component';
import { api } from '../../constants';
import { KargoModel } from '../../models/kargo.model';
import { FlexiToastService } from 'flexi-toast';
import { ResultModel } from '../../models/result.model';
import { CommonModule } from '@angular/common';
import { FlexiButtonComponent } from 'flexi-button';
import { FlexiPopupModule } from 'flexi-popup';
import { FormsModule } from '@angular/forms';


@Component({
  imports: [RouterLink, FlexiGridModule, BlankComponent, CommonModule, FlexiButtonComponent, FlexiPopupModule, FormsModule],
  templateUrl: './kargolar.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})

export default class KargolarComponent {
//resource, Angular'daki veri yükleme işlemi için kullanılır.
//request kısmı, bileşenin mevcut durumunu (state) alıyor.
//t8oken = signal<string>("eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjhmZGU1ODhkLTVmZGQtNGJhMS1hMjgxLTc3MzhkODBkNTQxNiIsIm5iZiI6MTc0MTY3NjQ4OCwiZXhwIjoxNzQxNzYyODg4LCJpc3MiOiJUYW5lciBTYXlkYW0iLCJhdWQiOiJUYW5lciBTYXlkYW0ifQ.nSYGA_LnUEnuw4FeDe1niERfT1jHABByR55it1Ni8gH4mMM73xWsja9C9YTKzSSThb7ZhmZBqI1ZxjEA-TfkKg") 
result = resource({
  request: () => this.state(),
  loader: async () => {
    console.log("Loader çalıştı mı?");
    let endpoint = `${api}/odata/kargolar?$count=true`;
    const odataEndpoint = this.#grid.getODataEndpoint(this.state());
    endpoint += "&" + odataEndpoint;
      const res = await lastValueFrom(this.#http.get<ODataModel<any[]>>(endpoint));
      return res;

  }
});

readonly data = computed(() => this.result.value()?.value ?? []);
readonly total = computed(() => this.result.value()?.['@odata.count'] ?? 0);
readonly loading = linkedSignal(() => this.result.isLoading());
isPopupVisible=false;
readonly popupLoading=signal<boolean>(false);
readonly state = signal<StateModel>(new StateModel());
readonly durumFilterData = signal<FlexiGridFilterDataModel[]>([
  {name: "Bekliyor", value: 0},
  {name: "Araca Teslim Edildi", value: 1},
  {name: "Yola Çıktı", value: 2},
  {name: "Teslim Şubesine Ulaştı", value: 3},
  {name: "Teslim İçin Yola Çıktı", value: 4},
  {name: "Teslim Edildi", value: 5},
  {name: "Adreste Kimse Bulunamadı", value: 6},
  {name: "İptal Edildi", value: 7}
]);
readonly durumUpdateRequest = signal<{id:string , durumValue:number}>({id:"",durumValue:0});

#http = inject(HttpClient);//tokeni ekleyebilmek için httpClient'e ihtiyacımız var
#grid = inject(FlexiGridService);
#breadcrumb = inject(BreadcrumbService);
#toast = inject(FlexiToastService);

constructor(){
  this.#breadcrumb.reset();
  this.#breadcrumb.add("Kargolar","/kargolar","package_2");
}


dataStateChange(event: StateModel){
  this.state.set(event);
}

async exportExcel(){
  let endpoint= `${api}/odata/kargolar?$count=true`;
  var res = await lastValueFrom(this.#http.get<ODataModel<any[]>>(endpoint));
  this.#grid.exportDataToExcel(res.value,"Kargo Listesi");
}

//item KargoModel kullanmamızın sebebi silme işlemi yapılmadan önce soru sorduracağız
delete(item: KargoModel){
  const endpoint = `${api}/kargolar/${item.id}`;
  this.#toast.showSwal("Kargoyu Sil?", `Aşağıdaki bilgilere ait kargoyu silmek istiyor musunuz?<br/><b>Gönderen: </b>
    ${item.gonderenFullName} <br/><b> Alıcı:</b> ${item.aliciFullName}`,()=> {
      this.loading.set(true);
      this.#http.delete<ResultModel<string>>(endpoint).subscribe(res =>{
      this.#toast.showToast("Bilgilendirme",res.data!,"info");
      this.result.reload();
      });
    })
}

getDurumClass(durum: string){
  if(durum === "Bekliyor"){
    return "alert-warning"
  }
  return "";
}

getAgirlikTotal(){
  const agirliklar = this.data().map(p => p.agirlik);
  let total = 0;
  agirliklar.forEach(e => total += e);
  return total;
}

//kargo durum popunu açma metodu
openUpdateDurumPopup(item:KargoModel){
this.durumUpdateRequest().id = item.id;
this.durumUpdateRequest().durumValue = item.durumDurumValue;
this.isPopupVisible=true;
}

//Kargo duurmunu güncelleme metodu
updateDurum(){
this.popupLoading.set(true);
this.#http.put<ResultModel<string>>(`${api}/kargolar/update-status`, this.durumUpdateRequest()).subscribe(res=>{
  this.popupLoading.set(false);
  this.isPopupVisible=false;
  this.result.reload(); //result listemizi reload ile tekrar çekiyor
  this.#toast.showToast("Başarılı",res.data!,"info");
})
}
}
