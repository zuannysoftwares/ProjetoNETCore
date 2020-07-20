import { Component, OnInit, TemplateRef } from '@angular/core';
import { EventoService } from '../_services/evento.service';
import { Evento } from '../_models/Evento';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { ptBrLocale} from 'ngx-bootstrap/locale';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';

defineLocale('pt-br', ptBrLocale);

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css']
})
export class EventosComponent implements OnInit {

  title = 'Lista de Eventos';

  eventosFiltrados: Evento[];
  eventos: Evento[];
  evento: Evento;
  modoSalvar = 'post';
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;
  registerForm: FormGroup;
  bodyDeletarEvento = '';

  vr: string;

  file: File;
  fileNameToUpdate: string;

  // registerForm é o nome do meu formulário com as validações
  // BsModalRef é o nome da modal
  // Encapsulamento
  // tslint:disable-next-line: variable-name
  _filtroLista = '';

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private toastr: ToastrService
    ) {
      this.localeService.use('pt-br'); // usado para formatar o datepicker dd/MM/yyyy
    }

    get filtroLista(): string{
      return this._filtroLista;
    }
    set filtroLista(value: string){
      this._filtroLista = value;
      this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
    }

    editarEvento(evento: Evento, template: any){
      this.modoSalvar = 'put';
      this.openModal(template);
      this.evento = Object.assign({}, evento); // Copia o evento passado por parâmetro para o evento já carregado para edição
      this.fileNameToUpdate = evento.imageUrl.toString();
      this.evento.imageUrl = '';
      this.registerForm.patchValue(this.evento);
    }

    novoEvento(template: any){
      this.modoSalvar = 'post';
      this.openModal(template);
    }

    excluirEvento(evento: Evento, template: any){
      this.openModal(template);
      this.evento = evento;
      this.bodyDeletarEvento = `Tem certeza que deseja excluir o Evento: ${evento.tema}, Código: ${evento.id} ?`;
    }

    confirmarDelete(template: any){
      this.eventoService.deleteEvento(this.evento.id).subscribe(
        () => {
          template.hide();
          this.getEventos();
          this.toastr.success('Evento excluído com sucesso');
        }, error => {
          this.toastr.error('Não foi possível excluir o evento');
          console.log(error);
        }
      );
    }

    openModal(template: any){
      this.registerForm.reset();
      template.show();
    }

    ngOnInit() {
      this.validation(); // validações do formulário
      this.getEventos(); // Retorna os eventos cadastrados na base de dados
    }

    filtrarEventos(filtrarPor: string): Evento[] {
      filtrarPor = filtrarPor.toLocaleLowerCase();
      return this.eventos.filter(
        evento => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1
        );
    }

    alternarImagem(){
        this.mostrarImagem = !this.mostrarImagem;
    }

    onFileChange(event){
      const reader = new FileReader();

      if (event.target.files && event.target.files.length){
        this.file = event.target.files;
      }
    }

    uploadImagem(){
      if (this.modoSalvar === 'post'){
        const nomeArquivo = this.evento.imageUrl.split('\\', 3);
        this.evento.imageUrl = nomeArquivo[2];
        this.eventoService.postUpload(this.file, nomeArquivo[2]).subscribe(
          () => {
            this.vr = new Date().getMilliseconds().toString();
            this.getEventos();
          }
        );
      }else{
        this.evento.imageUrl = this.fileNameToUpdate;
        this.eventoService.postUpload(this.file, this.fileNameToUpdate).subscribe(
          () => {
            this.vr = new Date().getMilliseconds().toString();
            this.getEventos();
          }
        );
      }
    }

    salvarAlteracao(template: any){
      if (this.registerForm.valid){
        if (this.modoSalvar === 'post'){
          this.evento = Object.assign({}, this.registerForm.value);

          this.uploadImagem();

          this.eventoService.postEvento(this.evento).subscribe(
            (novoEvento: Evento) => {
              console.log(novoEvento);
              template.hide();
              this.getEventos();
              this.toastr.success('Evento Salvo com sucesso!');
            }, error => {
              this.toastr.error(`Não foi possível salvar o Evento: ${error}`);
              console.log(error);
            }
          );
        } else {
          this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);

          this.uploadImagem();

          this.eventoService.putEvento(this.evento).subscribe(
            (novoEvento: Evento) => {
              console.log(novoEvento);
              template.hide();
              this.getEventos();
              this.toastr.success('Evento Atualizado com sucesso!');
            }, error => {
              this.toastr.error(`Não foi possível Editar o Evento: ${error}`);
              console.log(error);
            }
          );
        }
      }
    }
    validation(){
      this.registerForm = this.fb.group({
        tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
        local: ['', Validators.required],
        dataEvento: ['', Validators.required],
        qtdPessoas: ['', [Validators.required, Validators.max(500)]],
        imageUrl: ['', Validators.required],
        telefone: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]]
      });
    }
    getEventos(){
      this.vr = new Date().getMilliseconds().toString();

      this.eventoService.getAllEvento().subscribe(
        ( _eventos: Evento[] ) => {
          this.eventos = _eventos;
        }, error => {
          this.toastr.error(`Não foi possível Carregar os Eventos: ${error}`);
          console.log(error);
        });
      }
    }
