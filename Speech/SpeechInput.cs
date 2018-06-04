﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SpeechInput.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "SpeechInput.cs" was last formatted by Protiguous on 2018/06/04 at 4:25 PM.

namespace Librainian.Speech {

	using System;
	using System.Globalization;
	using System.Linq;
	using System.Speech.Recognition;
	using Collections;
	using Parsing;

	public class SpeechInput {

		public Lazy<Grammar> Grammar { get; } = new Lazy<Grammar>( () => {
			var grammar = new DictationGrammar { Enabled = true };

			return grammar;
		} );

		public Lazy<SpeechRecognitionEngine> RecognitionEngine { get; }

		public SpeechInput() =>
			this.RecognitionEngine = new Lazy<SpeechRecognitionEngine>( () => {
				var speechRecognitionEngine = new SpeechRecognitionEngine( CultureInfo.CurrentCulture );

				try { speechRecognitionEngine.LoadGrammar( this.Grammar.Value ); }
				catch ( InvalidOperationException ) { }

				try { speechRecognitionEngine.SetInputToDefaultAudioDevice(); }
				catch ( InvalidOperationException ) { "Warning: No microphone found.".Warning(); }

				try { speechRecognitionEngine.RecognizeAsync( RecognizeMode.Multiple ); }
				catch ( InvalidOperationException ) { }

				return speechRecognitionEngine;
			}, isThreadSafe: true );

		public static Grammar CreateGrammars( params String[] phrases ) {
			if ( phrases is null ) { throw new ArgumentNullException( nameof( phrases ) ); }

			var choices = new Choices( phrases );
			var builder = new GrammarBuilder( choices );

			return new Grammar( builder );
		}

		public void AttachEvent( Action<AudioLevelUpdatedEventArgs> audioLevelUpdated = null, Action<AudioSignalProblemOccurredEventArgs> audioSignalProblemOccurred = null,
			Action<AudioStateChangedEventArgs> audioStateChanged = null, Action<LoadGrammarCompletedEventArgs> loadGrammarCompleted = null, Action<RecognizeCompletedEventArgs> recognizeCompleted = null,
			Action<RecognizerUpdateReachedEventArgs> recognizerUpdateReached = null, Action<SpeechDetectedEventArgs> speechDetected = null, Action<SpeechHypothesizedEventArgs> speechHypothesized = null,
			Action<SpeechRecognitionRejectedEventArgs> speechRecognitionRejected = null, Action<SpeechRecognizedEventArgs> speechRecognized = null ) {
			this.RecognitionEngine.Value.AudioLevelUpdated += ( sender, args ) => audioLevelUpdated?.Invoke( args );
			this.RecognitionEngine.Value.AudioSignalProblemOccurred += ( sender, args ) => audioSignalProblemOccurred?.Invoke( args );
			this.RecognitionEngine.Value.AudioStateChanged += ( sender, args ) => audioStateChanged?.Invoke( args );
			this.RecognitionEngine.Value.LoadGrammarCompleted += ( sender, args ) => loadGrammarCompleted?.Invoke( args );
			this.RecognitionEngine.Value.RecognizeCompleted += ( sender, args ) => recognizeCompleted?.Invoke( args );
			this.RecognitionEngine.Value.RecognizerUpdateReached += ( sender, args ) => recognizerUpdateReached?.Invoke( args );
			this.RecognitionEngine.Value.SpeechDetected += ( sender, args ) => speechDetected?.Invoke( args );
			this.RecognitionEngine.Value.SpeechHypothesized += ( sender, args ) => speechHypothesized?.Invoke( args );
			this.RecognitionEngine.Value.SpeechRecognitionRejected += ( sender, args ) => speechRecognitionRejected?.Invoke( args );
			this.RecognitionEngine.Value.SpeechRecognized += ( sender, args ) => speechRecognized?.Invoke( args );
		}

		/// <summary>
		///     <seealso cref="AttachEvent" />
		/// </summary>
		/// <param name="action"></param>
		public void OnRecognizeSentence( Action<String> action ) =>
			this.RecognitionEngine.Value.SpeechRecognized += ( s, args ) => {
				var words = args.Result.Words.Select( unit => unit.Text ).ToList();
				var sentence = words.ToStrings( ParsingExtensions.Singlespace, "." );
				action( sentence );
			};

		public void Stop() => this.RecognitionEngine.Value.RecognizeAsyncCancel();
	}
}